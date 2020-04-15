using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Mango.Captcha.Core.Models;
using MangoMis.Frame.Helper.RedisV2;

namespace Mango.Captcha.Core
{
    public class MangoCaptcha
    {
        private static readonly Dictionary<Guid, IList<int>> SelectValues;
        private static readonly IList<CaptchaCategory> CaptchaCategories;
        private static readonly IList<CaptchaImage> CaptchaImages;
        private static DateTime _lasTime;
        private static string _imageRepoPath;
        private static readonly RedisClientPoolManager RedisClientPoolManager;

        static MangoCaptcha()
        {
            RedisClientPoolManager = MangoRedisHelper.GetDefault();
            SelectValues = new Dictionary<Guid, IList<int>>();
            CaptchaCategories = new List<CaptchaCategory>();
            CaptchaImages = new List<CaptchaImage>();
        }


        /// <summary>
        /// 初始化验证码库
        /// </summary>
        /// <param name="imageRepoPath">图片库地址</param>
        public void Init(string imageRepoPath)
        {
            if (!Directory.Exists(imageRepoPath))
            {
                throw new Exception("验证码图片库路径有误");
            }
            _imageRepoPath = imageRepoPath;
            DirectoryInfo directoryInfo = new DirectoryInfo(imageRepoPath);

            foreach (var dir in directoryInfo.GetDirectories())
            {
                var category = new CaptchaCategory
                {
                    Name = dir.Name,
                    Images = new List<CaptchaImage>()

                };
                foreach (var fileInfo in dir.GetFiles())
                {
                    var tmp = new CaptchaImage
                    {
                        Id = Guid.NewGuid(),
                        Path = fileInfo.FullName,
                        Category = category
                    };
                    CaptchaImages.Add(tmp);
                    category.Images.Add(tmp);
                }
                CaptchaCategories.Add(category);
            }

            _lasTime = DateTime.Now;
        }


        public Image Generate(out Guid key)
        {
            if (_lasTime.AddHours(24) <= DateTime.Now)
            {
                Init(_imageRepoPath);
            }

            if (CaptchaCategories == null || CaptchaCategories.Count == 0)
            {
                throw new Exception("图片库路径配置不正确！");
            }

            if (CaptchaImages == null || CaptchaImages.Count == 0)
            {
                throw new Exception("未在图片库中发现图片！");
            }
            //todo:过滤两条以上的分类

            var c1 = CaptchaCategories.Where(a => a.Images.Count > 1).ToList();

            var c2 = RandomSortList(c1).ToList();

            var images = new List<CaptchaImage>();

            var c3 = RandomSortList(c2[0].Images.ToList());


            for (int i = 0; i < 2; i++)
            {
                images.Add(new CaptchaImage
                {
                    IsCheck = true,
                    Id = c3[i].Id,
                    Path = c3[i].Path
                });

            }
            var allImages = RandomSortList(CaptchaImages.Where(a => a.Category != c2[0]).ToList());

            for (int i = 0; i < 7; i++)
            {
                images.Add(allImages[i]);
            }

            key = Guid.NewGuid();

            images = RandomSortList(images);

            IList<int> selectValues = new List<int>();

            for (int i = 0; i < images.Count; i++)
            {
                if (images[i].IsCheck)
                {
                    selectValues.Add(i);
                }
            }

            SelectValues.Add(key, selectValues);

            RedisClientPoolManager.AddDataSetListWithExpire("mango:captcha:" + key, selectValues.Select(a => a.ToString()).ToList(), 30);

            try
            {
                return MergeImage(images.Select(a => a.Path).ToList(), "请选择图中所有的" + c2[0].Name);
            }
            catch (Exception e)
            {
                Init(_imageRepoPath);
                return Generate(out key);
            }
        }


        /// <summary>
        /// 校验图形验证码
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="selectValues"></param>
        /// <returns></returns>
        public bool Validate(Guid key, IList<int> selectValues)
        {

            var result = RedisClientPoolManager.GetDataSet("mango:captcha:" + key);
            var trueValues = result.Result;
            if (result.ResultType == RedisResultType.OutOfTimeRange)
            {
                throw new Exception("验证码已经过期");
            }

            if (result.ResultType== RedisResultType.Failed)
            {
                throw new Exception("非法的验证码");
            }

            RedisClientPoolManager.Delete("mango:captcha:" + key);
            if (trueValues.Count != selectValues.Count)
                return false;
            foreach (var value in selectValues)
            {
                if (!trueValues.Contains(value.ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        public string GetScript()
        {
            var script = typeof(MangoCaptcha).Assembly.GetManifestResourceStream("Mango.Captcha.Core.assets.mango-captcha.js");
            string strScript;
            using (StreamReader streamReader = new StreamReader(script ?? throw new InvalidOperationException("mango-captcha.js未找到"), Encoding.UTF8))
            {
                strScript = streamReader.ReadToEnd();
            }
            return strScript;
        }

        /// <summary>
        /// 乱序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<T> RandomSortList<T>(List<T> list)
        {
            //Random random = new Random();
            var random = new Random(Guid.NewGuid().GetHashCode());
            List<T> newList = new List<T>();
            foreach (T item in list)
            {
                newList.Insert(random.Next(newList.Count + 1), item);
            }
            return newList;
        }

        private static Image MergeImage(List<string> images, string text)
        {
            var m1 = MergeProvider.Merge9Images(images, text);
            var image = MergeProvider.ConvertToImage(m1);
            return image;

        }

    }
}

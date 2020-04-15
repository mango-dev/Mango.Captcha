var css = `
        .mango-captcha-full {
            width: 344px;
            height: 384px;
            display: flex;
            flex-wrap: wrap;
            padding: 0;
        }

        .mango-captcha-part {
            width: 33.3333%;
            height: 114px;
        }

         .select {
            background-image: url(data:image/png;base64,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);
            background-size: 50%;
            background-repeat: no-repeat;
            background-position: center;
        }

        .mango-captcha-text {
            width: 100%;
            height: 40px;
        }

        .mango-captcha-btn {
            height: 30px;
            width: 80px;
        }`
var selectValues = [];
var currentKey = '';
var mangoCaptcha = {
    loadImage: function (imageUrl, key) {
        clearCaptcha();
        document.getElementsByClassName('mango-captcha-full')[0].style.background = `url(${imageUrl})`
        // $(".mango-captcha-full").css("background", `url(${imageUrl})`)
        currentKey = key
    }
};

HTMLElement.prototype.initCaptcha = function (fn, options) {
    // $('head').append(css);
    var head = document.head || document.getElementsByTagName('head')[0];
    var style = document.createElement('style');
    style.innerHTML = css;
    head.appendChild(style);
    var el = this;

    $(el).addClass("mango-captcha-full")
    var domHtml = "";
    for (var i = 0; i < 9; i++) {
        domHtml += `<div class="mango-captcha-part" data-id="${i}"></div>`
    }
    domHtml += ` <div class="mango-captcha-text"></div>
    <input id="btn-mango-captcha-refresh" type="button" class="mango-captcha-btn" value="刷新"> 
    <input id="btn-mango-captcha-ok" type="button" class="mango-captcha-btn" value="确定">`
    $(el).append(domHtml)
    $(".mango-captcha-part").click(function () {
        var id = Number.parseInt($(this).attr("data-id"))
        // alert($(this).attr("data-id"))
        if ($(this).hasClass("select")) {
            $(this).removeClass("select")
            selectValues = selectValues.filter(a => a != id)
        } else {
            $(this).addClass("select")
            selectValues.push(id)
        }
    })

    $("#btn-mango-captcha-refresh").click(function () {
        clearCaptcha();
        var myEvent = new CustomEvent('captcha_refresh', {
            detail: {
                a: '11111'
            },
        });
        if (el.dispatchEvent) {
            el.dispatchEvent(myEvent);
        } else {
            el.fireEvent(myEvent);
        }
    })

    $("#btn-mango-captcha-ok").click(function () {
        var myEvent = new CustomEvent('captcha_ok', {
            detail: {
                selectValues: selectValues,
                key: currentKey
            }
        });
        if (el.dispatchEvent) {
            el.dispatchEvent(myEvent);
        } else {
            el.fireEvent(myEvent);
        }
    })
    fn.call(el, mangoCaptcha)
}

function clearCaptcha() {
    selectValues = [];
    $(".mango-captcha-part").removeClass('select')
}



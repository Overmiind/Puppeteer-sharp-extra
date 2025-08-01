## Recaptcha plugin

Solves visible captcha automatically with one single line of code! Can handle captcha with button and callback.

## Usage  

```c#

// Initialize recaptcha plugin with AntiCaptchaProvider
var recaptchaPlugin = new RecaptchaPlugin(new AntiCaptcha("MyToken"));
var browser = await new PuppeteerExtra().Use(recaptchaPlugin).LaunchAsync(new LaunchOptions()
{
  Headless = true
});

var page = await browser.NewPageAsync();
await page.GoToAsync("https://patrickhlauke.github.io/recaptcha/");
// Solves captcha in page!
await recaptchaPlugin.SolveCaptchaAsync(page);
            
```

#### Providers 

🤖 [AntiCaptcha](https://anti-captcha.com/mainpage)

👾 [2captcha](https://2captcha.com/ru)

You can use your own provider implements IRecaptcha provider interface who should return g-recaptcha-responce. 

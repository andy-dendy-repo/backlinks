using Backlinks_LE.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backlinks_LE.SearchTools
{
    public class WebDriver : SearchBase
    {
        private object _locker = new object();
        private List<BoxIWebDriver> _drivers = new List<BoxIWebDriver>();
        
        public void Quit()
        {
            foreach (var w in _drivers)
                w.Driver.Quit();
        }
        public WebDriver(int threads, FreeThread handler) : base(threads,handler)
        {
            Init();
        }
        private void Init()
        {
            for (int i = 0; i < _threads; i++)
            {
                IWebDriver driver;
                //if (i <= _threads/2)
                //{
                //    ChromeOptions options = new ChromeOptions();
                //    options.AddArguments(new List<string>() { "headless", "disable-gpu" });
                //    var chromeDriverService = ChromeDriverService.CreateDefaultService();
                //    chromeDriverService.HideCommandPromptWindow = true;
                //    driver = new ChromeDriver(chromeDriverService, options);
                //}
                //else
                //{
                //    FirefoxOptions options = new FirefoxOptions();
                //    options.AddArguments(new List<string>() { "--headless" });
                //    var firefoxDriverService = FirefoxDriverService.CreateDefaultService();
                //    firefoxDriverService.HideCommandPromptWindow = true;
                //    driver = new FirefoxDriver(firefoxDriverService, options);
                //}
                ChromeOptions options = new ChromeOptions();
                options.AddArguments(new List<string>() { "headless", "disable-gpu" });
                var chromeDriverService = ChromeDriverService.CreateDefaultService();
                chromeDriverService.HideCommandPromptWindow = true;
                driver = new ChromeDriver(chromeDriverService, options);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                BoxIWebDriver box = new BoxIWebDriver(driver);
                _drivers.Add(box);
            }
        }

        

        protected override async Task<string> GetResp(string url)
        {
            string respT = string.Empty;

            BoxIWebDriver box;


            await Task.Run(() => {
                while (true)
                    lock (_locker)
                    {
                        box = _drivers.FirstOrDefault(x => x.Free == true);
                        if (box != null)
                        {
                            box.Free = false;
                            break;
                        }
                    }

                IWebDriver driver = box.Driver;
                Actions action = new Actions(driver);
                action.MoveByOffset(5, 5).MoveByOffset(10, 15).MoveByOffset(20, 15);
                try
                {
                    driver.Navigate().GoToUrl(url);
                    action.Perform();
                    respT = driver.PageSource;
                    box.Free = true;
                }
                catch
                {
                    box.Free = true;
                }
            });
            return respT;
        }

        class BoxIWebDriver
        {
            public IWebDriver Driver;
            public bool Free = true;

            public BoxIWebDriver(IWebDriver driver)
            {
                Driver = driver;
            }

        }
    }
}

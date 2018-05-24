using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ConceptFlower
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static Dictionary<string, object> Dic = new Dictionary<string, object>();
        protected override void OnStartup(StartupEventArgs e)
        {
           
          
           
            //base.OnStartup(e);

            Thread t = new Thread(() =>
            {
                SplashScreen s = new SplashScreen("pic.jpg");
                Dic["SplashWindow"] = s;//储存
                s.Show(true);//不能用Show
                Thread.Sleep(10000);
            });
            t.SetApartmentState(ApartmentState.STA);//设置单线程
            t.Start();

            base.OnStartup(e);



        }


    }
}

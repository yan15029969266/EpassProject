using ConceptFlower.BLL;
using ConceptFlower.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptFlower.Static
{
    public static class XMLProcesser
    {
        public static string ReadXML(string path)
        {
            return new StreamReader(path).ReadToEnd();
        }

        public static NewRequest GetEntry<NewRequest>(string xml)
        {
            return (NewRequest)XMLUtil.Deserialize(typeof(NewRequest), xml);
        }

        public static NewRequest GetObject<NewRequest>(string path)
        {
            return GetEntry<NewRequest>(ReadXML(path));
        }

        public static void ParseHowToRun(IList<NewRequest> list)
        {
            foreach (var item in list)
            {
                if (item.Header.MessageRef.Equals("A"))
                {
                    DoAction1();
                }
                else
                {
                    DoAction2();
                }
            }
        }


        public static void DoAction1()
        {
            //Write excel
            //ExcelHandler aaa = new ExcelHandler();
            //aaa.WriteSth("sdfsdfsdf");

            //process for AS400.
            new PCOMProcess().Page1().Page2().Next();
        }

        public static void DoAction2()
        {
            new PCOMProcess().Next().GotoPage(1);
        }

        public static void DoAction3(string base64)
        {
          
        }

        public static void Play(string path)
        {
            DoAction3(GetEntry<NewRequest>(ReadXML(path)).
                TransferCase.FirstOrDefault().FormImageCode);
        }

    }
}

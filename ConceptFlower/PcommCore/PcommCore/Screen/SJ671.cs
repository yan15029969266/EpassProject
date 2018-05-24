using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PcommCore.Screen
{
   public  class SJ671 :ScreenLogic
    {


        public ScreenDes screenDes = new ScreenDes();

        public List<string> Message { get; set; }

        public SJ671()
        {
            Message = new List<string>();
            ContentTag tag = new ContentTag("SJ671", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }

        public void SearchSchemeID(string schemeId)

        {

            base.SetText("S",21,69);

            base.SetText(schemeId,22,13);

            base.Enter(22,13);

        }


        public void SetPrintALL(string value="S",int row=21,int col=69)

        {
            base.SetText(value, row, col);

        }

        public void SetStoAll()
        {
            base.SetText("S", 21, 69);
        }

        public void ClearSchemeID()
        {
           var strlen= base.GetText(22,13,20).Trim();
            SetCursorPos(22,13);
            for (int i =0;i < strlen.Length;i++)
            {
                base.SendKey(KeyBoard.Delete);
            }
        }


        public bool loopSecheme_id(List<string> slit,int row,PcommCore pc) 
        {

            bool keyword = false;

            if (string.IsNullOrEmpty(base.ReadRow(row)))

            {

                Message.Add("row data is empty");
               // throw new Exception("row data is empty");
            }
            else
            {
                base.SetText("Y", row, 2);
                base.Enter();
                row++;

                if (row == 20)
                {
                    //throw new Exception("read data was completed");
                    Message.Add("row data is empty");

                }
                pc.LinkToScreen<SJ672>((SJ672) =>
                {
                    var str = SJ672.getContent();
                    slit.ForEach(x => {
                        if (str.Contains(x)&&!string.IsNullOrEmpty(x))
                        {
                            keyword = true;
                            Message.Add(x + "key word is exsits");
                            base.SendKey(KeyBoard.PF3);
                            return;
                            //throw new Exception(x + "key word is exsits");     
                            //throw new Exception(x + "key word is exsits");
                        }
                        //else
                        //{
                        //    base.SendKey(KeyBoard.PF3);
                        //}
                    });
                    base.SendKey(KeyBoard.PF3);
                    return true;
                });
               
            }
             return keyword;
        }


        public bool SelectSchemeID(List<string> slit,int row, PcommCore pc)
        {
            bool isKeyword = false;

            if (base.NotepadHeaderIsHaveNext())
            {
                isKeyword= loopSecheme_id(slit, row, pc);
                if (isKeyword == true)
                {
                    return true;
                }
                else
                {
                    base.PageDown();
                    SelectSchemeID(slit, row, pc);
                }

            }
            else if (base.NotepadHeaderIsBottom())
            {
                isKeyword= loopSecheme_id(slit, row, pc);
            }

            return isKeyword;
        }

        public bool NotePadPass(List<string> slit, PcommCore pc)
        {
            bool pass = true;
            Thread.Sleep(1000);
            if(string.IsNullOrEmpty(GetText(10, 4, 8).Trim()))
            {
                return pass;
            }
            if (base.NotepadHeaderIsHaveNext())
            {
                for(int row=10; row < 20; row++)
                {
                    string schemeID = GetText(row, 4, 8).Trim();
                    if(string.IsNullOrEmpty(schemeID))
                    {
                        break;
                    }
                    else
                    {
                        base.SetText("Y", row, 2);
                    }
                }
                base.PageDown();
                NotePadPass(slit, pc);
            }
            else if (base.NotepadHeaderIsBottom())
            {
                for (int row = 10; row < 19; row++)
                {
                    string schemeID = GetText(row, 4, 8).Trim();
                    if (string.IsNullOrEmpty(schemeID))
                    {
                        break;
                    }
                    else
                    {
                        base.SetText("Y", row, 2);
                    }
                }
            }
            SendKey(KeyBoard.Enter);
            pass=CheckNotepad(slit,pc);
            return pass;
        }

        private bool CheckNotepad(List<string> slit, PcommCore pc)
        {
            bool pass = true;
            pc.LinkToScreen<SJ672>((SJ672) =>
            {
                var str = SJ672.getContent().ToUpper();
                foreach(string s in slit)
                {
                    if(str.Contains(s.ToUpper()) &&!string.IsNullOrEmpty(s))
                    {
                        pass = false;
                        break;
                    }
                }
                base.SendKey(KeyBoard.PF3);
                return true;
            });
            if(!pass)
            {
                pc.SkipToHomeScreen<S0017>();
            }
            else
            {
                CommonScreen comm = pc.GetScreen<CommonScreen>();
                string screenCode = comm.GetText(1, 72, 80);
                if (screenCode.Contains("SJ672"))
                {
                    CheckNotepad(slit, pc);
                }
            }
            return pass;
        }
    }
}

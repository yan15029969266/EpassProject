/*
* ==============================================================================
*
* File name: ContentTag
* Description: Base on Personal Communications for Windows, Version 6.0
* Host Access Class Library
*
* Version: 1.0
* Created: 12/21/2017 4:33:29 PM
*
* Author: Haley X L Zhang
* Company: Chinasoft International
*
* ==============================================================================
*/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PcommCore.Common
{
    public class ScreenDes
    {
        private List<ContentTag> tagList;
        private dynamic screenDesc = null;
        Type screenDescType = Type.GetTypeFromProgID("PCOMM.autECLScreenDesc");
        public ScreenDes()
        {
            screenDesc = Activator.CreateInstance(screenDescType);
            tagList = new List<ContentTag>();
        }
        public string Name { get; set; }
        public List<ContentTag> TagList
        {
            get
            {
                return tagList;
            }
        }
        public object ScreenDesc
        {
            get
            {
                return screenDesc;
            }
        }
        public void AddTag(ContentTag tag)
        {
            screenDesc.AddString( tag.Content, tag.StartRow, tag.StartColumn, tag.CaseSense );
            tagList.Add(tag);
        }
        public void AddTags(List<ContentTag> tags)
        {
            foreach (ContentTag tag in tags)
            {
                AddTag(tag);
            }
        }
    }
    public class ContentTag
    {
        private int startRow;
        private int startColumn;
        private int endRow;
        private int endColumn;
        private string contnet;
        private bool caseSense;
        public ContentTag(string contnet, int startRow, int startColumn, int endRow, int endColumn, bool caseSense = false)
        {
            this.startRow = startRow;
            this.startColumn = startColumn;
            this.endRow = endRow;
            this.endColumn = endColumn;
            this.contnet = contnet;
            this.caseSense = caseSense;
        }
        public int StartRow
        {
            get
            {
                return startRow;
            }
        }
        public int StartColumn
        {
            get
            {
                return startColumn;
            }
        }
        public int EndRow
        {
            get
            {
                return endRow;
            }
        }
        public int EndColumn
        {
            get
            {
                return endColumn;
            }
        }
        public string Content
        {
            get
            {
                return contnet;
            }
        }
        public bool CaseSense
        {
            get
            {
                return caseSense;
            }
        }
    }
}

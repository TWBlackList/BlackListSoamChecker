﻿using System.Text.RegularExpressions;
using BlackListSoamChecker.DbManager;

// 这是迷之 Spam Message Checker

namespace BlackListSoamChecker.CommandObject
{
    internal class SpamMessageKeyword
    {
        public string GetEqualsKeyword(SpamMessageObj[] spamMessages, string text) // Mode 0 完全匹配
        {
            string totalPoints = "";
            foreach (SpamMessageObj msg in spamMessages)
                if (text.ToLower().Equals(msg.Message.ToLower()))
                    totalPoints = totalPoints + msg.Message.ToLower() + " : " + msg.Point + "\n";
            return totalPoints;
        }

        public string GetRegexKeyword(SpamMessageObj[] spamMessages, string text) // Mode 1 正则
        {
            string totalPoints = "";
            foreach (SpamMessageObj msg in spamMessages)
                if (new Regex(msg.Message).Match(text).Success)
                    totalPoints = totalPoints + msg.Message + " : " + msg.Point + "\n";
            return totalPoints;
        }

        public string GetSpamKeyword(SpamMessageObj[] spamMessages, string text) // Mode 2 迷之算法
        {
            string totalPoints = ""; // 总分，预定义，返回值用
            int textLen = text.Length - 1; // 被检测的消息的长度
            foreach (SpamMessageObj msg in spamMessages) // 已有的关键字循环
            {
                string targetStr = msg.Message; // 关键字
                int targetMsgLen = msg.Message.Length; // 关键字长度
                int lastPath = 0; // 最后一次检测消息时关键字所在长度
                int skipTo = 0;
                for (int nowPath = 0; nowPath < textLen; nowPath++) // 被检测消息被打断循环
                {
                    if (nowPath < skipTo) continue;
                    if (text[nowPath] == targetStr[lastPath])
                    {
                        // 如果被检测的消息的当前字符和当前关键字的字符匹配，则将关键字位置 +1
                        lastPath++;
                    }
                    else if (lastPath != 0) // 如果最后一次检测消息时关键字所在长度不是 0，则检查被检查消息的下一个字是否和当前关键字字符匹配
                    {
                        if (text[nowPath + 1] == targetStr[lastPath])
                        {
                            // 如果匹配则跳过两个字，并且将关键字位置 +1
                            skipTo = nowPath + 2;
                            lastPath++;
                        }
                    }
                    else
                    {
                        lastPath = 0;
                    }

                    if (lastPath >= targetMsgLen)
                    {
                        // 如果当前关键字位置超出范围则代表完全匹配，则加分

                        totalPoints = totalPoints + "迷之算法 : " + msg.Point + "\n";
                        break;
                    }
                }
            }

            return totalPoints;
        }

        public string GetIndexOfKeyword(SpamMessageObj[] spamMessages, string text) // Mode 3 寻找匹配字符串
        {
            string totalPoints = "";
            foreach (SpamMessageObj msg in spamMessages)
                if (text.ToLower().IndexOf(msg.Message.ToLower()) != -1)
                    totalPoints = totalPoints + msg.Message.ToLower() + " : " + msg.Point + "\n";
            return totalPoints;
        }

        public string GetHalalKeyword(string text) // Mode 4 清真
        {
            string totalPoints = "";
            int textLen = text.Length - 1;
            for (int nowPath = 0; nowPath < textLen; nowPath++)
            {
                char nowChar = text[nowPath];
                string unicode = System.Convert.ToInt32(nowChar).ToString("X4");

                if (nowChar >= 0x0600 && nowChar <= 0x06FF)
                {
                    totalPoints = totalPoints + nowChar + "  : " + unicode + " : 1 (0600-06FF)\n";
                    continue;
                }

                if (nowChar >= 0x0750 && nowChar <= 0x077F)
                {
                    totalPoints = totalPoints + nowChar + "  : " + unicode + " : 1 (0750-077F)\n";
                    continue;
                }

                if (nowChar >= 0x08A0 && nowChar <= 0x08FF)
                {
                    totalPoints = totalPoints + nowChar + "  : " + unicode + " : 1 (08A0-08FF)\n";
                    continue;
                }
                
                if (nowChar >= 0xFB50 && nowChar <= 0xFDFF)
                {
                    totalPoints = totalPoints + nowChar + "  : " + unicode + " : 1 (BF50-FDFF)\n";
                    continue;
                }
                
                if (nowChar >= 0x1EE00 && nowChar <= 0x1EEFF) totalPoints = totalPoints + nowChar + "  : " + unicode + " : 1 (1EE00-1EEFF)\n";
                
            }

            return totalPoints;
        }

        public string GetIndiaKeyword(string text) // Mode 5 印度
        {
            string totalPoints = "";
            int textLen = text.Length - 1;
            for (int nowPath = 0; nowPath < textLen; nowPath++)
            {
                char nowChar = text[nowPath];
                string unicode = System.Convert.ToInt32(nowChar).ToString("X4");
                
                if (nowChar >= 0x0900 && nowChar <= 0x097F)
                {
                    totalPoints = totalPoints + nowChar + "  : " + unicode + " : 1 (0900-097F)\n";
                    continue;
                }

                if (nowChar >= 0x1CD0 && nowChar <= 0x1CFF)
                {
                    totalPoints = totalPoints + nowChar + "  : " + unicode + " : 1 (1CD0-1CFF)\n";
                    continue;
                }

                if (nowChar >= 0xA8E0 && nowChar <= 0xA8FF) totalPoints = totalPoints + nowChar + "  : " + unicode + " : 1 (A8E0-A8FF)\n";
                
            }

            return totalPoints;
        }

        public string GetContainsKeyword(SpamMessageObj[] spamMessages, string text) // Mode 6 如果包含
        {
            string totalPoints = "";
            int point = 0;
            foreach (SpamMessageObj msg in spamMessages)
                if (text.ToLower().Contains(msg.Message.ToLower()))
                {
                    point = msg.Point * (text.ToLower().Split(msg.Message.ToLower()).Length - 1);
                    totalPoints = totalPoints + msg.Message.ToLower() + " : " + point + "\n";
                }

            return totalPoints;
        }

        public string GetRussiaKeyword(string text) // Mode 7 普丁
        {
            string totalPoints = "";
            int textLen = text.Length - 1;
            for (int nowPath = 0; nowPath < textLen; nowPath++)
            {
                char nowChar = text[nowPath];
                string unicode = System.Convert.ToInt32(nowChar).ToString("X4");
                
                if (nowChar >= 0x0400 && nowChar <= 0x04FF)
                {
                    totalPoints = totalPoints + nowChar + "  : " + unicode + " : 1 (0400-04FF)\n";
                    continue;
                }
                
                if (nowChar >= 0x0500 && nowChar <= 0x052F)
                {
                    totalPoints = totalPoints + nowChar + "  : " + unicode + " : 1 (0500-052F)\n";
                    continue;
                }
                
                if (nowChar >= 0x2DE0 && nowChar <= 0x2DEF)
                {
                    totalPoints = totalPoints + nowChar + "  : " + unicode + " : 1 (2DE0-2DEF)\n";
                    continue;
                }
                if (nowChar >= 0xA640 && nowChar <= 0xA69F) totalPoints = totalPoints + nowChar + "  : " + unicode + " : 1 (A640-A69F)\n";
            }

            return totalPoints;
        }
        public string GetNameKeyword(SpamMessageObj[] spamMessages, string name) // Mode 8 Name
        {
            string totalPoints = "";
            int point = 0;
            foreach (SpamMessageObj msg in spamMessages)
                if (name.ToLower().Contains(msg.Message.ToLower()))
                {
                    point = msg.Point * (name.ToLower().Split(msg.Message.ToLower()).Length - 1);
                    totalPoints = totalPoints + msg.Message.ToLower() + " : " + point + "\n";
                }

            return totalPoints;
        }
    }
}
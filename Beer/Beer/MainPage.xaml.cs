using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Beer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            CalBeer();
        }


        private void CalBeer()
        {
            int total = 0;
            int lineNo = 0;

            int beerPrice = 2;
            int capUnit = 4;
            int emptyUnit = 2;

            int beerNumber = 10 / 2;
            total = beerNumber;

            WrileLine(lineNo, total, beerNumber, 0, 0,0,0);

            lineNo++;
            int capNumber = beerNumber / capUnit;
            int capLeftNumber = beerNumber % capUnit;
            int emptyNumber = beerNumber / emptyUnit;
            int emptyLeftNumber = beerNumber % emptyUnit;
            beerNumber = capNumber + emptyNumber;
            total += beerNumber;

            WrileLine(lineNo, total, beerNumber, capNumber, capLeftNumber, emptyNumber, emptyLeftNumber);
           
            while (beerNumber>0)
            {
                capNumber = beerNumber / capUnit;
                capLeftNumber += beerNumber % capUnit;
                if (capLeftNumber >= capUnit)
                {
                    capNumber += capLeftNumber / capUnit;
                    capLeftNumber = capLeftNumber % capUnit;
                }

                emptyNumber = beerNumber / emptyUnit;
                emptyLeftNumber += beerNumber % emptyUnit;
                if (emptyLeftNumber >= emptyUnit)
                {
                    emptyNumber += emptyLeftNumber / emptyUnit;
                    emptyLeftNumber = emptyLeftNumber % emptyUnit;
                }

                lineNo++;
                beerNumber = capNumber + emptyNumber;
                total += beerNumber;

                WrileLine(lineNo, total, beerNumber, capNumber, capLeftNumber, emptyNumber, emptyLeftNumber);
            }
        }

        private void WrileLine(int lineNo,int total,int beerNumber, int capNumber, int capLeftNumber, int emptyNumber, int emptyLeftNumber)
        {
            Paragraph para = new Paragraph();

            para.Inlines.Add(new Run() { Text = $"{lineNo}   " });
            para.Inlines.Add(new Run() { Text = $"(盖子:{capNumber})+(空瓶:{emptyNumber})= (啤酒:{beerNumber})   " });
            para.Inlines.Add(new Run() { Text = $"累计:{total}   " });

            para.Inlines.Add(new Run() { Text = $"剩余盖子={capLeftNumber},剩余空瓶={emptyLeftNumber}" });
           
            para.Inlines.Add(new LineBreak());

            rtbOutput.Blocks.Add(para);
        }
    }
}

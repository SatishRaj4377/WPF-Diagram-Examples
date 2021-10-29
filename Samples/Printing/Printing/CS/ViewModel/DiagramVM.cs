#region Copyright Syncfusion Inc. 2001-2019.
// Copyright Syncfusion Inc. 2001-2019. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using GettingStarted_Print.Utility;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Diagram;
using Syncfusion.Windows.Shared.Printing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PageOrientation = Syncfusion.UI.Xaml.Diagram.PageOrientation;

namespace GettingStarted_Print.ViewModel
{
    public class DiagramVM : DiagramViewModel
    {
        #region fields

        private PrintPageSize _pagesize;
        private PrintPageMargin _margin;
        private string _printzoom;
        private List<PrintPageMargin> _mMargin = new List<PrintPageMargin>
        {
            new PrintPageMargin()
            {
                MarginName = "Normal",
                Thickness = new Thickness(94.488188976375,94.488188976375,94.488188976375,94.488188976375),
            },
            new PrintPageMargin()
            {
                MarginName = "Narrow",
                Thickness = new Thickness(47.9999999999985,47.9999999999985,47.9999999999985,47.9999999999985),
            },
            new PrintPageMargin()
            {
                MarginName = "Moderate",
                Thickness = new Thickness(72.1889763779505,95.999999999997,72.1889763779505,95.999999999997),
            },
            new PrintPageMargin()
            {
                MarginName = "Wide",
                Thickness = new Thickness(191.999999999994,95.999999999997,191.999999999994,95.999999999997),
            },
            new PrintPageMargin()
            {
                MarginName = "Custom Margins",
                Thickness = new Thickness(94.5,94.5,94.5,94.5),
            },
        };
        private bool CustomMarginChanged = false;
        private bool _portraitorientation = true;
        private bool _landscapeorientation = false;
        private bool _singlepage = false;
        private bool _multiplepage = true;
        private List<PrintPageSize> _mPageSizes = new List<PrintPageSize>
        {
            new PrintPageSize
            {
                PageSizeName = "Letter",
                Size = new Size(816,1056),
                PageSizeUnit = PageSizeUnit.Inches,
            },
            new PrintPageSize
            {
                PageSizeName = "Folio",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(816,1296),
            },
            new PrintPageSize
            {
                PageSizeName = "Legal",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(816,1344),
            },
            new PrintPageSize
            {
                PageSizeName = "Ledger",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(1056,1632),
            },
            new PrintPageSize
            {
                PageSizeName = "A0",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(3179,4494),
            },
            new PrintPageSize
            {
                PageSizeName = "A1",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(2245,3179),
            },
            new PrintPageSize
            {
                PageSizeName = "A2",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(1587,2245),
            },
            new PrintPageSize
            {
                PageSizeName = "A3",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(1123,1587),
            },
            new PrintPageSize
            {
                PageSizeName = "A4",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(794,1123),
            },
            new PrintPageSize
            {
                PageSizeName = "A5",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(559,794),
            },
            new PrintPageSize
            {
                PageSizeName = "ANSI A",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(816,1056),
            },
            new PrintPageSize
            {
                PageSizeName = "ANSI B",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(1056,1632),
            },
            new PrintPageSize
            {
                PageSizeName = "ANSI C",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(1632,2112),
            },
            new PrintPageSize
            {
                PageSizeName = "ANSI D",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(2112,3264),
            },
            new PrintPageSize
            {
                PageSizeName = "ANSI E",
                PageSizeUnit = PageSizeUnit.Inches,
                Size = new Size(3264,4226),
            },
            new PrintPageSize()
            {
                PageSizeName = "Unknown",
                PageSizeUnit = PageSizeUnit.Inches,
            },
        };
        private List<string> _mPrintZoom = new List<string> { "400%", "200%", "100%", "50%", "25%" };
        
        #endregion

        #region Constructor
        public DiagramVM()
        {
            //Initialize Methods
            InitializeSfDiagram();

            PrintingCommand = new Command(OnPrintingCommand);
            PortVisibility = PortVisibility.Visible;
            PrintingService.PrintManager.SelectedScaleIndex = 1;
            //Custom Command to execute Print action     
            PrintClickCommand = new Command(OnPrintCommand);
            PageBreakCommand = new Command(OnPageBreakCommand);
            SkipEmptyPagesCommand = new Command(OnSkipEmptyPagesCommand);
            OrientationCommand = new Command(OnOrientationCommand);
            PagesCommand = new Command(OnPagesCommand);

            PageSettings = new CustomPageSettings(this)
            {
                PageWidth = 816,
                PageHeight = 1056,
                ShowPageBreaks = true,
                MultiplePage = true,
                PageOrientation = PageOrientation.Portrait,
                PageBackground = new SolidColorBrush(Colors.White),
                PrintMargin = new Thickness(94.5, 94.5, 94.5, 94.5),
                Unit = new LengthUnit() { Unit = LengthUnits.Pixels },
            };
        }

        /// <summary>
        /// Method to initialize diagram print properties
        /// </summary>
        private void InitializeSfDiagram()
        {
            //Initializing PrintingService for DiagramViewModel
            PrintingService = new CustomPrintingService(this);

            //to display page number at top of the print preview.
            PrintingService.PrintSettings.PageHeaderHeight = 50;
            PrintingService.PrintSettings.PageHeaderTemplate = App.Current.MainWindow.Resources["PrintHeaderTemplate"] as DataTemplate;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Method to change the pageorientation
        /// </summary>
        /// <param name="pageOrientation"></param>
        internal void OnPageOrientationChanged(PageOrientation pageOrientation)
        {
            if (pageOrientation == PageOrientation.Portrait)
            {
                PortraitOrientation = true;
                LandScapeOrientation = false;
            }
            else
            {
                PortraitOrientation = false;
                LandScapeOrientation = true;
            }
        }

        /// <summary>
        /// Method to get the PrintingEventArgs to get the printstate and add papar sizes to required printer.
        /// </summary>
        /// <param name="obj"></param>
        private void OnPrintingCommand(object obj)
        {
            var args = obj as PrintingEventArgs;
            // The below code is to add page size for printing
            if (args.PrintState == PrintStatus.Started)
            {
                var customPages = new System.Collections.Generic.Dictionary<string, Size>();
                var printerName = args.PrintDialog.PrintQueue.Name;
                if (printerName.Contains("Microsoft Print to PDF"))
                {
                    customPages.Add("Ansi B", new Size(1055, 1632));
                    customPages.Add("Ansi C", new Size(1632, 2112));
                    customPages.Add("Ansi D", new Size(2112, 3264));
                    customPages.Add("A0", new Size(3179, 4494));
                }
                else if (printerName.Contains("Microsoft XPS Document Writer"))
                {
                    customPages.Add("A0", new Size(3179, 4494));
                    customPages.Add("A1", new Size(2245, 3179));
                }

                foreach (var customPage in customPages)
                {
                    if (args.SelectedPageMediaSizeName.Contains(customPage.Key))
                    {
                        var pageSize = customPage.Value;
                        var mediaSize = new PageMediaSize(PageMediaSizeName.Unknown, pageSize.Width, pageSize.Height);
                        args.PrintDialog.PrintTicket.PageMediaSize = mediaSize;
                        args.CanUseCustomPageMediaSize = true;
                        break;
                    }
                }
            }

            if (args.PrintState == PrintStatus.Cancelled)
            {
                MessageBox.Show("Printing Cancelled");
            }
        }

        /// <summary>
        /// Method to set the multiple page or single page while printing
        /// </summary>
        /// <param name="obj"></param>
        private void OnPagesCommand(object obj)
        {
            string content = obj.ToString();
            switch (content)
            {
                case "Multiple":
                    PrintingService.PrintManager.SelectedScaleIndex = 1;
                    Multiplepage = true;
                    Singlepage = false;
                    break;
                case "Single":
                    PrintingService.PrintManager.SelectedScaleIndex = 0;
                    Multiplepage = false;
                    Singlepage = true;
                    break;
            }
        }

        /// <summary>
        /// Internal method to change the page size accordingly with the size changes in print preview window.
        /// </summary>
        /// <param name="pageHeight"></param>
        /// <param name="pageWidth"></param>
        internal void OnPageSizesChanged(double pageHeight, double pageWidth)
        {
            bool isMatch = false;
            foreach (PrintPageSize pagesize in PageSizes)
            {
                if (PageSettings.PageOrientation == PageOrientation.Portrait)
                {
                    if (pagesize.Size.Height == Math.Round(pageHeight) && pagesize.Size.Width == Math.Round(pageWidth))
                    {
                        PageSize = pagesize;
                        isMatch = true;
                        break;
                    }
                }
                else if (this.PageSettings.PageOrientation == PageOrientation.Landscape)
                {
                    if (pagesize.Size.Height == Math.Round(pageWidth) && pagesize.Size.Width == Math.Round(pageHeight))
                    {
                        PageSize = pagesize;
                        isMatch = true;
                        break;
                    }
                }
            }

            if (!isMatch && pageHeight.ToString() != "NaN" && pageWidth.ToString() != "NaN")
            {
                PageSize = PageSizes.Last();
                PageSize.Size = new Size(pageWidth, pageHeight);
            }        
        }

        /// <summary>
        /// Method to change the scale index
        /// </summary>
        /// <param name="scaleindex"></param>
        internal void OnScaleIndexChanged(int scaleindex)
        {
            if(scaleindex.ToString() == "1")
            {
                Multiplepage = true;
                Singlepage = false;
            }
            else
            {
                Multiplepage = false;
                Singlepage = true;
            }
        }

        /// <summary>
        /// Method to change the margin values
        /// </summary>
        /// <param name="printMargin"></param>
        internal void OnMarginChanged(Thickness printMargin)
        {
            bool IsMarginUpdated = false;
            foreach(PrintPageMargin printmargin in margin)
            {
                if(printMargin.Bottom == printmargin.Thickness.Bottom && printMargin.Right == printmargin.Thickness.Right
                    && printMargin.Top == printmargin.Thickness.Top && printMargin.Left == printmargin.Thickness.Left)
                {
                    if(printmargin.MarginName == "Custom Margins")
                    {
                        CustomMarginChanged = true;
                    }
                    Margin = printmargin;
                    IsMarginUpdated = true;
                    break;
                }
            }
            if(!IsMarginUpdated)
            {
                CustomMarginChanged = true;
                Margin = margin.Last();
                Margin.Thickness = printMargin;
            }
        }

        /// <summary>
        /// Method to change orientation
        /// </summary>
        /// <param name="obj"></param>
        private void OnOrientationCommand(object obj)
        {
            string content = obj.ToString();
            switch (content)
            {
                case "Portrait":
                    PageSettings.PageOrientation = PageOrientation.Portrait;
                    PortraitOrientation = true;
                    LandScapeOrientation = false;
                    break;
                case "Landscape":
                    PageSettings.PageOrientation = PageOrientation.Landscape;
                    PortraitOrientation = false;
                    LandScapeOrientation = true;
                    break;
            }
        }

        /// <summary>
        /// Method to change print zoom
        /// </summary>
        /// <param name="printzoom"></param>
        private void OnPrintZoomValueChanged(string printzoom)
        {
            string content = printzoom.ToString();
            switch (content)
            {
                case "400%":
                    PageSettings.PrintScale = 400 / 100;
                    break;
                case "200%":
                    PageSettings.PrintScale = 200 / 100;
                    break;
                case "100%":
                    PageSettings.PrintScale = 100 / 100;
                    break;
                case "50%":
                    PageSettings.PrintScale = 50 / 100;
                    break;
                case "25%":
                    PageSettings.PrintScale = 25 / 100;
                    break;
            }
        }
        /// <summary>
        /// Method to update margin for digram.
        /// </summary>
        /// <param name="margin"></param>
        private void OnMarginValueChanged(PrintPageMargin margin)
        {
            string content = margin.MarginName;
            switch (content)
            {
                case "Normal":
                    PageSettings.PrintMargin = new Thickness(94.488188976375, 94.488188976375, 94.488188976375, 94.488188976375);
                    break;
                case "Narrow":
                    PageSettings.PrintMargin = new Thickness(47.9999999999985, 47.9999999999985, 47.9999999999985, 47.9999999999985);
                    break;
                case "Moderate":
                    PageSettings.PrintMargin = new Thickness(72.1889763779505, 95.999999999997, 72.1889763779505, 95.999999999997);
                    break;
                case "Wide":
                    PageSettings.PrintMargin = new Thickness(191.999999999994, 95.999999999997, 191.999999999994, 95.999999999997);
                    break;
                case "Custom Margins":
                    if (!CustomMarginChanged)
                    {
                        CustomMargin custom = new CustomMargin();
                        custom.Owner = App.Current.MainWindow;
                        custom.ShowDialog();
                        if (custom.PrintMarginLeftUPDowm.Text != "" && custom.PrintMarginRightUPDowm.Text != "" &&
                            custom.PrintMarginBottomUPDowm.Text != "" && custom.PrintMarginTopUPDowm.Text != "")
                        {
                            PageSettings.PrintMargin = new Thickness(double.Parse(custom.PrintMarginLeftUPDowm.Text.ToString()),
                                                                     double.Parse(custom.PrintMarginTopUPDowm.Text.ToString()),
                                                                     double.Parse(custom.PrintMarginRightUPDowm.Text.ToString()),
                                                                     double.Parse(custom.PrintMarginBottomUPDowm.Text.ToString()));
                            margin.Thickness = PageSettings.PrintMargin;
                        }
                    }
                    else
                    {
                        CustomMarginChanged = false;
                    }
                    break;
            }
        }

        /// <summary>
        /// Method to change page size
        /// </summary>
        /// <param name="pagesize"></param>
        private void OnPageSizeValueChanged(PrintPageSize pagesize)
        {
            string content = pagesize.PageSizeName;
            if (PageSettings != null)
            {
                if (PageSettings.PageOrientation == PageOrientation.Portrait)
                {
                    switch (content)
                    {
                        case "Letter":
                            PageSettings.PageWidth = 816;
                            PageSettings.PageHeight = 1056;
                            break;
                        case "Legal":
                            PageSettings.PageWidth = 816;
                            PageSettings.PageHeight = 1344;
                            break;
                        case "Folio":
                            PageSettings.PageWidth = 816;
                            PageSettings.PageHeight = 1296;
                            break;
                        case "Ledger":
                            PageSettings.PageWidth = 1056;
                            PageSettings.PageHeight = 1632;
                            break;
                        case "A0":
                            PageSettings.PageWidth = 3179;
                            PageSettings.PageHeight = 4494;
                            break;
                        case "A1":
                            PageSettings.PageWidth = 2245;
                            PageSettings.PageHeight = 3179;
                            break;
                        case "A2":
                            PageSettings.PageWidth = 1587;
                            PageSettings.PageHeight = 2245;
                            break;
                        case "A3":
                            PageSettings.PageWidth = 1123;
                            PageSettings.PageHeight = 1587;
                            break;
                        case "A4":
                            PageSettings.PageWidth = 794;
                            PageSettings.PageHeight = 1123;
                            break;
                        case "A5":
                            PageSettings.PageWidth = 559;
                            PageSettings.PageHeight = 794;
                            break;
                        case "ANSI A":
                            PageSettings.PageWidth = 816;
                            PageSettings.PageHeight = 1056;
                            break;
                        case "ANSI B":
                            PageSettings.PageWidth = 1056;
                            PageSettings.PageHeight = 1632;
                            break;
                        case "ANSI C":
                            PageSettings.PageWidth = 1632;
                            PageSettings.PageHeight = 2112;
                            break;
                        case "ANSI D":
                            PageSettings.PageWidth = 2112;
                            PageSettings.PageHeight = 3264;
                            break;
                        case "ANSI E":
                            PageSettings.PageWidth = 3264;
                            PageSettings.PageHeight = 4226;
                            break;
                    }
                }
                else
                {
                    switch (content)
                    {
                        case "Letter":
                            PageSettings.PageHeight = 816;
                            PageSettings.PageWidth = 1056;
                            break;
                        case "Legal":
                            PageSettings.PageHeight = 816;
                            PageSettings.PageWidth = 1344;
                            break;
                        case "Folio":
                            PageSettings.PageHeight = 816;
                            PageSettings.PageWidth = 1296;
                            break;
                        case "Ledger":
                            PageSettings.PageHeight = 1056;
                            PageSettings.PageWidth = 1632;
                            break;
                        case "A0":
                            PageSettings.PageHeight = 3179;
                            PageSettings.PageWidth = 4494;
                            break;
                        case "A1":
                            PageSettings.PageHeight = 2245;
                            PageSettings.PageWidth = 3179;
                            break;
                        case "A2":
                            PageSettings.PageHeight = 1587;
                            PageSettings.PageWidth = 2245;
                            break;
                        case "A3":
                            PageSettings.PageHeight = 1123;
                            PageSettings.PageWidth = 1587;
                            break;
                        case "A4":
                            PageSettings.PageHeight = 794;
                            PageSettings.PageWidth = 1123;
                            break;
                        case "A5":
                            PageSettings.PageHeight = 559;
                            PageSettings.PageWidth = 794;
                            break;
                        case "ANSI A":
                            PageSettings.PageHeight = 816;
                            PageSettings.PageWidth = 1056;
                            break;
                        case "ANSI B":
                            PageSettings.PageHeight = 1056;
                            PageSettings.PageWidth = 1632;
                            break;
                        case "ANSI C":
                            PageSettings.PageHeight = 1632;
                            PageSettings.PageWidth = 2112;
                            break;
                        case "ANSI D":
                            PageSettings.PageHeight = 2112;
                            PageSettings.PageWidth = 3264;
                            break;
                        case "ANSI E":
                            PageSettings.PageHeight = 3264;
                            PageSettings.PageWidth = 4226;
                            break;
                    }
                }
            }
        }
       
        /// <summary>
        /// Method to execute Print action
        /// </summary>
        /// <param name="obj"> Argument of the print command</param>
        private void OnPrintCommand(object obj)
        {
            PrintingService.ShowDialog = true;
            PrintingService.Print();
        }

        /// <summary>
        /// Method to change page breaks
        /// </summary>
        /// <param name="obj"></param>
        private void OnPageBreakCommand(object obj)
        {
            bool ischecked = bool.Parse(obj.ToString());
            if (ischecked)
            {
                PageSettings.ShowPageBreaks = true;
            }
            else
            {
                PageSettings.ShowPageBreaks = false;
            }
        }

        /// <summary>
        /// Method to decide skip empty pages.
        /// </summary>
        /// <param name="obj"></param>
        private void OnSkipEmptyPagesCommand(object obj)
        {
            bool ischecked = bool.Parse(obj.ToString());
            if (ischecked)
            {
                if (PrintingService is CustomPrintingService)
                {
                    (PrintingService as CustomPrintingService).skipEmptyPages = true;
                }
            }
            else
            {
                if (PrintingService is CustomPrintingService)
                {
                    (PrintingService as CustomPrintingService).skipEmptyPages = false;
                }
            }
        }

        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the print command.
        /// </summary>
        public ICommand PrintClickCommand { get; set; }
        /// <summary>
        /// Gets or sets the page break command.
        /// </summary>
        public ICommand PageBreakCommand { get; set; }
        /// <summary>
        /// Gets or sets the skip empty page command.
        /// </summary>
        public ICommand SkipEmptyPagesCommand { get; set; }
        /// <summary>
        /// Gets or sets the Orientation command.
        /// </summary>
        public ICommand OrientationCommand { get; set; }
        /// <summary>
        /// Gets or sets the Page size command.
        /// </summary>
        public ICommand PagesCommand { get; set; }

        /// <summary>
        /// Gets or sets the value indicates whether the portaitOrientation is enable or not.
        /// </summary>
        public bool PortraitOrientation
        {
            get
            {
                return _portraitorientation;
            }
            set
            {
                if(_portraitorientation != value)
                {
                    _portraitorientation = value;
                    OnPropertyChanged("PortraitOrientation");
                }
            }
        }

        /// <summary>
        /// Gets or sets the value indicates whether the LandScapeOrientation is enable or not.
        /// </summary>
        public bool LandScapeOrientation
        {
            get
            {
                return _landscapeorientation;
            }
            set
            {
                if (_landscapeorientation != value)
                {
                    _landscapeorientation = value;
                    OnPropertyChanged("LandScapeOrientation");
                }
            }
        }
        /// <summary>
        /// Gets or sets the value indicates whether the Multiplepage is true or false.
        /// </summary>
        public bool Multiplepage
        {
            get
            {
                return _multiplepage;
            }
            set
            {
                if (_multiplepage != value)
                {
                    _multiplepage = value;
                    OnPropertyChanged("Multiplepage");
                }
            }
        }
        /// <summary>
        /// Gets or sets the value indicates whether the Singlepage is true or false.
        /// </summary>
        public bool Singlepage
        {
            get
            {
                return _singlepage;
            }
            set
            {
                if (_singlepage != value)
                {
                    _singlepage = value;
                    OnPropertyChanged("Singlepage");
                }
            }
        }

        /// <summary>
        /// Gets or sets the PrintPageMargin.
        /// </summary>
        public PrintPageMargin Margin
        {
            get
            {
                return _margin;
            }
            set
            {
                if (_margin != value)
                {
                    _margin = value;
                    OnPropertyChanged("Margin");
                    OnMarginValueChanged(_margin);
                }
            }
        }

        /// <summary>
        /// Gets or sets the PageSize.
        /// </summary>
        public PrintPageSize PageSize
        {
            get
            {
                return _pagesize;
            }
            set
            {
                if (_pagesize != value)
                {
                    _pagesize = value;
                    OnPropertyChanged("PageSize");
                    OnPageSizeValueChanged(_pagesize);
                }
            }
        }

        /// <summary>
        /// Gets or sets the current zoomlevel of the diagram.
        /// </summary>
        public string PrintZoom
        {
            get
            {
                return _printzoom;
            }
            set
            {
                if (_printzoom != value)
                {
                    _printzoom = value;
                    OnPropertyChanged("PrintZoom");
                    OnPrintZoomValueChanged(_printzoom);
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of zooming.
        /// </summary>
        public List<string> printzoom
        {
            get
            {
                return _mPrintZoom;
            }
        }

        /// <summary>
        /// Gets or sets the list of PageSizes.
        /// </summary>
        public List<PrintPageSize> PageSizes
        {
            get
            {
                return _mPageSizes;
            }
        }
        /// <summary>
        /// Gets or sets the list of print margin.
        /// </summary>
        public List<PrintPageMargin> margin
        {
            get
            {
                return _mMargin;
            }
        }

        #endregion
    }
    /// <summary>
    ///  Informs windows that an application wishes to participate in Syncfusion.Windows.Controls.Printing.PrintManager.
    ///  The PrintManager class is also used for programmatically initiating printing.
    /// </summary>
    public class CustomDiagramPrintManager : DiagramPrintManager
    {
        public CustomDiagramPrintManager(PrintingService printingService, DiagramVM diagram) : base(printingService)
        {
            _diagram = diagram;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if(propertyName == "selectedScaleIndex")
            {                
                OnScaleIndexChanged(SelectedScaleIndex);
            }
        }
        /// <summary>
        /// Update single or multiple page of the diagram.
        /// </summary>
        /// <param name="selectedScaleIndex"></param>
        private void OnScaleIndexChanged(int selectedScaleIndex)
        {
            if(_diagram != null)
            {
                _diagram.OnScaleIndexChanged(selectedScaleIndex);
            }
        }

        DiagramVM _diagram;
        /// <summary>
        /// Add additional pagesize for pdf printer.
        /// </summary>
        /// <param name="printQueue"></param>
        public override void OnSelectedPrinterChanged(PrintQueue printQueue)
        {

            if (printQueue.Name.Contains("Microsoft Print to PDF"))
            {
                List<string> j = PageSizeOptions.Select(c => c.PageSizeName).ToList();
                if (!(j.Contains("Ansi B")))
                {
                    PageSizeOptions.Add(new Syncfusion.Windows.Shared.Printing.PrintPageSize() { PageSizeName = "Ansi B", Size = new Size(1055, 1632) });
                }
                if (!(j.Contains("Ansi C")))
                {
                    PageSizeOptions.Add(new Syncfusion.Windows.Shared.Printing.PrintPageSize() { PageSizeName = "Ansi C", Size = new Size(1632, 2112) });
                }
                if (!(j.Contains("Ansi D")))
                {
                    PageSizeOptions.Add(new Syncfusion.Windows.Shared.Printing.PrintPageSize() { PageSizeName = "Ansi D", Size = new Size(2112, 3264) });
                }
                if (!(j.Contains("A0")))
                {
                    PageSizeOptions.Add(new Syncfusion.Windows.Shared.Printing.PrintPageSize() { PageSizeName = "A0", Size = new Size(3179, 4494) });
                }
            }
        }
    }
    /// <summary>
    ///  Represent the class for printing service
    /// </summary>
    public class CustomPrintingService : PrintingService
    {
        DiagramVM _diagram;
        /// <summary>
        ///     Initializes a new instance of the CustomPrintingService class.
        /// </summary>
        /// <param name="diagram">instance of diagram</param>
        public CustomPrintingService(DiagramVM diagram)
        {
            _diagram = diagram;
            this.PrintManager = new CustomDiagramPrintManager(this,diagram);
        }

        public bool skipEmptyPages = false;
        /// <summary>
        /// To skip the empty pages in preview and printed document.
        /// </summary>
        /// <param name="args"></param>
        protected override void GetPrintInfo(PrintInfo args)
        {
            if (skipEmptyPages)
            {
                if (!(args.Elements as IEnumerable<object>).Any())
                {
                    args.Cancel = true;
                }
                else
                    base.GetPrintInfo(args);
            }
            else
            {
                base.GetPrintInfo(args);
            }
        }
    }

    public class PrintPageSize : INotifyPropertyChanged
    {
        #region Private Fields

        private string pageSizeName;
        private PageSizeUnit unit = PageSizeUnit.Inches;
        private Size size;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Unit for pagesize.
        /// </summary>
        public PageSizeUnit PageSizeUnit
        {
            get
            {
                return unit;
            }
            set
            {
                unit = value;
                RaisePropertyChanged("PageSizeUnit");
            }
        }
        /// <summary>
        /// Gets or sets the value to the page type.
        /// </summary>
        public string PageSizeName
        {
            get
            {
                return pageSizeName;
            }
            set
            {
                pageSizeName = value;
                RaisePropertyChanged("PageSizeName");
            }
        }
        private void RaisePropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Gets or sets the value to the page size.
        /// </summary>
        /// <remarks>
        /// Stores the size in cm.
        /// </remarks>
        public Size Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                RaisePropertyChanged("Size");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 
    }

    /// <summary>
    /// Represents the class <see cref="Syncfusion.Windows.Shared.Printing.PrintPageMargin"/> that contains the PageMarin informations.
    /// </summary>
    public class PrintPageMargin : INotifyPropertyChanged
    {
        #region Private Fields

        private Thickness thickness;
        private string marginName;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or set the value to the margin type.
        /// </summary>
        public string MarginName
        {
            get
            {
                return marginName;
            }
            set
            {
                marginName = value;
                RaisePropertyChanged("MarginName");
            }
        }

        private void RaisePropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Gets or sets the value to the thickness.
        /// </summary>
        public Thickness Thickness
        {
            get
            {
                return thickness;
            }
            set
            {
                thickness = value;
                RaisePropertyChanged("Thickness");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
    /// <summary>
    /// Represents to customize the appearance, width, and height of the Diagram page.
    /// </summary>
    public class CustomPageSettings : PageSettings
    {
        DiagramVM _diagram;
        /// <summary>
        ///     Initializes a new instance of the CustomPageSettings class.
        /// </summary>
        /// <param name="diagram">instance of diagram</param>
        public CustomPageSettings(DiagramVM diagram)
        {
            _diagram = diagram;
        }
        protected override void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(name);
            if (name == "PageWidth")
            {
                OnPageSizeChanged(PageHeight, PageWidth);
            }
            if (name == "PageHeight")
            {
                OnPageSizeChanged(PageHeight, PageWidth);
            }
            if(name == "PrintMargin")
            {
                OnMarginChanged(PrintMargin);
            }
            if(name == "PageOrientation")
            {
                OnOrientationChanged(PageOrientation);
            }
        }
        /// <summary>
        /// update the orientation of the page.
        /// </summary>
        /// <param name="pageOrientation">orientation of the page</param>
        private void OnOrientationChanged(PageOrientation pageOrientation)
        {
            if (_diagram.PageSettings != null)
            {
                _diagram.OnPageOrientationChanged(pageOrientation);
            }
        }
        /// <summary>
        /// update the margin of the page.
        /// </summary>
        /// <param name="printMargin"> page and print preview margin</param>
        private void OnMarginChanged(Thickness printMargin)
        {
            if (_diagram.PageSettings != null)
            {
                _diagram.OnMarginChanged(printMargin);
            }
        }
        /// <summary>
        /// update the size of the page.
        /// </summary>
        /// <param name="pageHeight">Height of the page</param>
        /// <param name="pageWidth">Width of the page</param>
        private void OnPageSizeChanged(double pageHeight, double pageWidth)
        {
            if (_diagram.PageSettings != null)
            {
                _diagram.OnPageSizesChanged(pageHeight, pageWidth);
            }
        }
    }
}

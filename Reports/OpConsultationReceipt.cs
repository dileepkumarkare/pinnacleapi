using DevExpress.XtraReports.UI;
using Pinnacle.Models;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace Pinnacle.Reports
{
    public partial class OpConsultationReceipt : DevExpress.XtraReports.UI.XtraReport
    {
        public OpConsultationReceipt()
        {
            InitializeComponent();
        }

        private void OpConsultationReceipt_BeforePrint(object sender, CancelEventArgs e)
        {

        }
        private void xrPictureBox1_BeforePrint(object sender, CancelEventArgs e)
        {
            XRPictureBox _xrPictureBox = sender as XRPictureBox;
            if (_xrPictureBox is not null)
            {
                string _logoName = GetCurrentColumnValue("Logo") as string;
                if (!string.IsNullOrEmpty(_logoName))
                {
                    string imageUrl = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "HospitalProfile", _logoName);
                    _xrPictureBox.ImageUrl = imageUrl;
                }
            }

        }
        private void xrRichText31_BeforPrint(object sender, CancelEventArgs e)
        {
            XRRichText xrRichText = sender as XRRichText;
            XRLabel xrLabel = this.FindControl("xrLabel21", true) as XRLabel;
            if (xrRichText is not null)
            {
                string amount = xrLabel.Text;
               
                if (!string.IsNullOrEmpty(amount))
                {
                    decimal _amount = Convert.ToDecimal(amount);
                    xrRichText.Text = MasterModel.AmountInWords(_amount);
                }
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Drawing;
using System.IO.Compression;
using System.EnterpriseServices;

namespace Dean
{

    // Tags for innernmost XML layer for deserialization
    [Serializable()]
    public class DateRange  // (used in ReportMetadata)
    {
        public int begin { get; set; }
        public int end { get; set; }
    }
    [Serializable()]
    public class PolicyPublished  // (used in ReportFeedback)
    {
        [XmlElement("domain")]
        public string domain { get; set; }
        [XmlElement("adkim")]
        public string adkim { get; set; }
        [XmlElement("aspf")]
        public string aspf { get; set; }
        [XmlElement("p")]
        public string p { get; set; }
        [XmlElement("sp")]
        public string sp { get; set; }
        [XmlElement("pct")]
        public string pct { get; set; }
    }
    [Serializable()]
    public class PolicyEvaluated  // (used in Row)
    {
        [XmlElement("disposition")]
        public string disposition { get; set; }
        [XmlElement("dkim")]
        public string dkim { get; set; }
        [XmlElement("spf")]
        public string spf { get; set; }
    }
    [Serializable()]
    public class Identifiers  // (used in Record)
    {
        [XmlElement("header_from")]
        public string headerFrom { get; set; }
    }
    [Serializable()]
    public class Dkim  // (used in AuthResults)
    {
        [XmlElement("domain")]
        public string domain { get; set; }
        [XmlElement("result")]
        public string result { get; set; }
        [XmlElement("selector")]
        public string selector { get; set; }
    }
    [Serializable()]
    public class Spf  // (used in AuthResults)
    {
        [XmlElement("domain")]
        public string domain { get; set; }
        [XmlElement("result")]
        public string result { get; set; }
    }

    // Tags for next outter XML layer
    [Serializable()]
    public class ReportMetadata  // (used in ReportFeedback)
    {
        [XmlElement("org_name")]
        public string orgName { get; set; }
        [XmlElement("email")]
        public string email { get; set; }
        // This is not always in the report
        [XmlElement("extra_contact_info")]
        public string extraContactInfo { get; set; }
        [XmlElement("report_id")]
        public string reportID { get; set; }
        [XmlElement("date_range")]
        public DateRange dateRange { get; set; }
    }
    [Serializable()]
    public class Row  // (used in Record)
    {
        [XmlElement("source_ip")]
        public string sourceIp { get; set; }
        [XmlElement("count")]
        public int count { get; set; }
        [XmlElement("policy_evaluated")]
        public PolicyEvaluated policyEvaluated { get; set; }
    }
    [Serializable()]
    public class AuthResults  // (used in Record)
    {
        [XmlElement("dkim", typeof(Dkim), IsNullable = true)]
        public Dkim[] dkims { get; set; }
        [XmlElement("spf")]
        public Spf spf { get; set; }
    }

    // Tags for next outter XML layer
    [Serializable()]
    public class Record  // (used in ReportFeedback)
    {
        [XmlElement("row")]
        public Row row { get; set; }
        [XmlElement("identifiers")]
        public Identifiers identifiers { get; set; }
        [XmlElement("auth_results")]
        public AuthResults authResults { get; set; }
    }

    // The outtermost XML tag (<feedback>)
    [Serializable()]
    [XmlRoot("ReportFeedback")]
    public class ReportFeedback
    {
        [XmlElement("report_metadata")]
        public ReportMetadata reportMetadata { get; set; }
        [XmlElement("policy_published")]
        public PolicyPublished policyPublished { get; set; }
        
        [XmlElement("record", typeof(Record))]
        public Record[] records { get; set; }
    }

    public partial class dmarc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }


         // Deserialize a single XML file and output the information in a structured and readable fashion
        protected void getInfo_Click(object sender, EventArgs e)
        {
            const string reportToDisplay = "report1.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(ReportFeedback), new XmlRootAttribute("feedback"));  // no xmls attribute for <feedback> tag, so add one

            string filePath = Server.MapPath($"/xmlFiles/{reportToDisplay}");
            Stream xmlReader = new FileStream(filePath, FileMode.Open);

            ReportFeedback report;
            report = (ReportFeedback)serializer.Deserialize(xmlReader);

            // Display the properties of the report in the left hand side literal of the page
            output1.Text = $"<h3><u>DMARC report contents for {reportToDisplay}</u>:</h3>";
            output1.Text += "<h5>Report metadata:</h5>";
            output1.Text += $"<p>    Org name: {report.reportMetadata.orgName}</p>";  // String interpolation makes life easier
            output1.Text += $"<p>    Email: {report.reportMetadata.email}</p>";
            output1.Text += $"<p>    Extra contact info: {report.reportMetadata.extraContactInfo}</p>";
            output1.Text += $"<p>    Report ID: {report.reportMetadata.reportID}</p>";
            output1.Text += $"<p>    Date: From {report.reportMetadata.dateRange.begin} to {report.reportMetadata.dateRange.end}</p>";
            output1.Text += "<h5>Policy published:</h5>";
            output1.Text += $"<p>    Domain: {report.policyPublished.domain}</p>";
            output1.Text += $"<p>    adkim: {report.policyPublished.adkim}</p>";
            output1.Text += $"<p>    aspf: {report.policyPublished.aspf}</p>";
            output1.Text += $"<p>    p: {report.policyPublished.p}</p>";
            output1.Text += $"<p>    sp: {report.policyPublished.sp}</p>";
            output1.Text += $"<p>    pct: {report.policyPublished.pct}</p>";
            output1.Text += "<h5>Report records:</h5>";
            int i = 0;
            foreach (Record r in report.records)
            {
                output1.Text += $"<p><b>Report {i}</b>:</p>";
                output1.Text += $"<p>    Count: {r.row.count}</p>";
                output1.Text += "<h6>    Policy evaluated:</h6>";
                output1.Text += $"<p>        Disposition: {r.row.policyEvaluated.disposition}</p>";
                output1.Text += $"<p>        dkim: {r.row.policyEvaluated.dkim}</p>";
                output1.Text += $"<p>        spf: {r.row.policyEvaluated.spf}</p>";
                output1.Text += $"<p>    Identifier header from: {r.identifiers.headerFrom}</p>";
                output1.Text += "<h6>    Auth results:</h6>";
                if (r.authResults.dkims != null)
                {
                    int j = 1;
                    foreach (Dkim d in r.authResults.dkims)
                    {
                        output1.Text += $"<h6>        dkim {j}:</h6>";
                        output1.Text += $"<p>            Domain: {d.domain}</p>";
                        output1.Text += $"<p>            Result: {d.result}</p>";
                        output1.Text += $"<p>            Selector: {d.selector}</p>";
                        j++;
                    }
                }
                output1.Text += "<h6>        spf:</h6>";
                output1.Text += $"<p>            Domain: {r.authResults.spf.domain}</p>";
                output1.Text += $"<p>            Result: {r.authResults.spf.result}</p>";
                i++;
            }

            literal1.Visible = true;

            xmlReader.Close();
        }


        // In right side Panel (literal2), display all fraudulent IP addresses in all the XML files in the directory
        protected void listIPs_Click(object sender, EventArgs e)
        {
            const string directory = "xmlFiles";
            string path = $"C:\\Users\\Dean Orenstein\\Documents\\dean-orenstein\\{directory}";
            
            DirectoryInfo xmlDir = new DirectoryInfo(path);
            FileInfo[] files = xmlDir.GetFiles("*.*");
            Dictionary <string, string> badIPsAndReasons = new Dictionary<string, string>();  // Holds "fraud IP:reason for it being there" pairs
            Dictionary<string, int> reportAndNumIPs = new Dictionary<string, int>();  // Holds "XML file:the number of unique fraud IPs it has" pairs

            // For each file, first deserialize the XML to ReportFeedback object
            int i = 1;            
            foreach (FileInfo file in files)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ReportFeedback), new XmlRootAttribute("feedback"));
                string filePath = Server.MapPath($"/{directory}/{file}");
                ReportFeedback currReport;
                Stream xmlReader = new FileStream(filePath, FileMode.Open);
                currReport = (ReportFeedback)serializer.Deserialize(xmlReader);

                // For each record in this file, get the bad IPs (<source_ip>)
                int numIPs = 0;
                foreach (Record record in currReport.records)
                {
                    string badIP = record.row.sourceIp;
                    // Ignore this IP if the dictionary already has it
                    if (badIPsAndReasons.ContainsKey(badIP))
                        continue;
                    numIPs++;
                    // See the reason for why the IP is fraud
                    string reason;
                    if (record.row.policyEvaluated.dkim == "fail")
                    {
                        if (record.row.policyEvaluated.spf == "fail")
                            reason = "Failed DKIM and SPF checks";
                        else
                            reason = "Failed DKIM check";
                    }
                    else if (record.row.policyEvaluated.spf == "fail")
                        reason = "Failed SPF check";
                    else  // Both checks pass -> IP's identified (count) > 1
                        reason = "Multiple IP's identified (count > 1)";
                    // Add ip:reason pair to our dictionary
                    badIPsAndReasons.Add(badIP, reason);                    
                }

                // Add file:num IPs pair to other dictionary
                reportAndNumIPs.Add(file.ToString(), numIPs);
                xmlReader.Close();
                i++;
            }

            // Add html elements to the right hand side literal (ID / IP / DNS name / Reason)
            output2.Text = $"<h2 id=\"numFound\">Found {badIPsAndReasons.Count} fraudulent IP addresses!</h2>";
            output2.Text += "<h3><u>IP ID / IP address / IP's DNS host name / Reason IP is fraudulent</u>:</h3>";
            output2.Text += "<h6>(DKIM stands for DomainKeys Identified Mail Protocol, SPF stands for Sender Policy Framework)<h6><br>";

            // Loop through the reports, adding HTML indicatig the report, and looping n times for each (n = number of fraud IPs in that report)
            int id = 0;
            foreach (string report in reportAndNumIPs.Keys)
            {
                output2.Text += "<p><b>" + report + "</b>:</p><br>";
                for (int ip = 0; ip < reportAndNumIPs[report]; ip++)
                {
                    string theIP = badIPsAndReasons.ElementAt(ip + id).Key;
                    string reason = badIPsAndReasons[theIP];

                    // Add more HTML for ID / IP / DNS name / Reason for this IP
                    output2.Text += $"<p>{ip + id} / {theIP} / ";
                    try
                    {
                        IPAddress formattedIP = IPAddress.Parse(theIP);
                        IPHostEntry DNSname = Dns.GetHostEntry(formattedIP);
                        output2.Text += $"{DNSname.HostName} / ";
                    }
                    catch (System.Net.Sockets.SocketException)
                    {
                        output2.Text += "Could not resolve this address. / ";
                    }

                    // No matter what happens, the reason for fraud is displayed and id is incremented
                    finally
                    {
                        output2.Text += $"{reason}</p><br>";
                    }
                }
                id += reportAndNumIPs[report];
            }

            literal2.Visible = true;
        }
    }
}
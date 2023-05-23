using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PaymentGateway.request.response
{
    public class xmlUtility
    {
        //handleSimplePaymentReport
        [XmlRoot(ElementName = "PaymentVO")]
        public class PaymentVO
        {

            [XmlElement(ElementName = "PaymentType")]
            public string PaymentType { get; set; }

            [XmlElement(ElementName = "Amount")]
            public int Amount { get; set; }

            [XmlElement(ElementName = "DoB")]
            public string DoB { get; set; }

            [XmlElement(ElementName = "Identifier")]
            public int Identifier { get; set; }

            [XmlElement(ElementName = "PayerInfo")]
            public object PayerInfo { get; set; }

            [XmlElement(ElementName = "ConsumerName")]
            public string ConsumerName { get; set; }

            [XmlElement(ElementName = "PaidDate")]
            public DateTime PaidDate { get; set; }

            [XmlElement(ElementName = "PaymentId")]
            public int PaymentId { get; set; }

            [XmlElement(ElementName = "Quantity")]
            public int Quantity { get; set; }

            [XmlElement(ElementName = "ReceiptCode")]
            public string ReceiptCode { get; set; }
        }

        [XmlRoot(ElementName = "PaymentArray")]
        public class PaymentArray
        {

            [XmlElement(ElementName = "PaymentVO")]
            public List<PaymentVO> PaymentVO { get; set; }

            public IEnumerator GetEnumerator()
            {
                return ((IEnumerable)PaymentVO).GetEnumerator();
            }
        }

        [XmlRoot(ElementName = "handleSimplePaymentReportResult", Namespace = "http://www.pay24-7.com/P247WS/PubMethods")]
        public class HandleSimplePaymentReportResult
        {
            [XmlElement(ElementName = "NumRecords")]
            public int NumRecords { get; set; }

            [XmlElement(ElementName = "SuccessState")]
            public int SuccessState { get; set; }

            [XmlElement(ElementName = "PaymentArray")]
            public PaymentArray PaymentArray { get; set; }

            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }

            [XmlText]
            public string Text { get; set; }
        }

        //handleMessageUpdateRequest
        [XmlRoot(ElementName = "handleMessageUpdateRequestResult", Namespace = "http://www.pay24-7.com/P247WS/PubMethods")]
        public class HandleMessageUpdateRequestResult
        {

            [XmlElement(ElementName = "SuccessState")]
            public int SuccessState { get; set; }

            [XmlElement(ElementName = "VerboseSuccessState")]
            public string VerboseSuccessState { get; set; }

            [XmlElement(ElementName = "UsersModified")]
            public int UsersModified { get; set; }
        }
    }
}
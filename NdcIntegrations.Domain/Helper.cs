using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NdcIntegrations.Domain
{
    public class Helper
    {
		public static T Deserialize<T>(string xml)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			using (var reader = new System.IO.StringReader(xml))
			{
				return (T)serializer.Deserialize(reader);
			}
		}
	}
}

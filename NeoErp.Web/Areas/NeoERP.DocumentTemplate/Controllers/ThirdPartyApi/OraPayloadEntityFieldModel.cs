using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace NeoERP.DocumentTemplate.Controllers.ThirdPartyApi
{
	// using System.Xml.Serialization;
	// XmlSerializer serializer = new XmlSerializer(typeof(OraPayloadEntity));
	// using (StringReader reader = new StringReader(xml))
	// {
	//    var test = (OraPayloadEntity)serializer.Deserialize(reader);
	// }

	[XmlRoot(ElementName = "OraPayloadEntityField")]
	public class OraPayloadEntityFieldModel
	{

		[XmlAttribute(AttributeName = "field")]
		public string Field { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		[XmlAttribute(AttributeName = "value")]
		public int Value { get; set; }

		[XmlElement(ElementName = "ValueList")]
		public ValueListModel ValueList { get; set; }
	}

	[XmlRoot(ElementName = "OraPayloadEntityFieldGenericParameter")]
	public class OraPayloadEntityFieldGenericParameterModel
	{

		[XmlAttribute(AttributeName = "field")]
		public string Field { get; set; }

		[XmlAttribute(AttributeName = "value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "GenericParameterList")]
	public class GenericParameterListModel
	{

		[XmlElement(ElementName = "OraPayloadEntityFieldGenericParameter")]
		public List<OraPayloadEntityFieldGenericParameterModel> OraPayloadEntityFieldGenericParameter { get; set; }
	}

	[XmlRoot(ElementName = "ValueList")]
	public class ValueListModel
	{

		[XmlElement(ElementName = "GenericParameterList")]
		public GenericParameterListModel GenericParameterList { get; set; }
	}

	[XmlRoot(ElementName = "FieldList")]
	public class FieldListModel
	{

		[XmlElement(ElementName = "OraPayloadEntityField")]
		public List<OraPayloadEntityFieldModel> OraPayloadEntityField { get; set; }
	}

	[XmlRoot(ElementName = "Header")]
	public class HeaderModel
	{

		[XmlElement(ElementName = "FieldList")]
		public FieldListModel FieldList { get; set; }

		[XmlAttribute(AttributeName = "Index")]
		public int Index { get; set; }

		[XmlAttribute(AttributeName = "LinkId")]
		public int LinkId { get; set; }
	}

	[XmlRoot(ElementName = "OraPayloadEntityMI")]
	public class OraPayloadEntityMIModel
	{

		[XmlElement(ElementName = "FieldList")]
		public FieldListModel FieldList { get; set; }

		[XmlAttribute(AttributeName = "Index")]
		public int Index { get; set; }

		[XmlAttribute(AttributeName = "LinkId")]
		public int LinkId { get; set; }
	}

	[XmlRoot(ElementName = "MenuItemList")]
	public class MenuItemListModel
	{

		[XmlElement(ElementName = "OraPayloadEntityMI")]
		public List<OraPayloadEntityMIModel> OraPayloadEntityMI { get; set; }
	}

	[XmlRoot(ElementName = "OraPayloadEntityTmed")]
	public class OraPayloadEntityTmedModel
	{

		[XmlElement(ElementName = "FieldList")]
		public FieldListModel FieldList { get; set; }

		[XmlAttribute(AttributeName = "Index")]
		public int Index { get; set; }

		[XmlAttribute(AttributeName = "LinkId")]
		public int LinkId { get; set; }
	}

	[XmlRoot(ElementName = "TenderMediaList")]
	public class TenderMediaListModel
	{

		[XmlElement(ElementName = "OraPayloadEntityTmed")]
		public OraPayloadEntityTmedModel OraPayloadEntityTmed { get; set; }
	}

	[XmlRoot(ElementName = "Check")]
	public class CheckModel
	{

		[XmlElement(ElementName = "Header")]
		public HeaderModel Header { get; set; }

		[XmlElement(ElementName = "MenuItemList")]
		public MenuItemListModel MenuItemList { get; set; }

		[XmlElement(ElementName = "DiscountList")]
		public object DiscountList { get; set; }

		[XmlElement(ElementName = "ServiceChargeList")]
		public object ServiceChargeList { get; set; }

		[XmlElement(ElementName = "ExtensibilityDataList")]
		public object ExtensibilityDataList { get; set; }

		[XmlElement(ElementName = "TenderMediaList")]
		public TenderMediaListModel TenderMediaList { get; set; }

		[XmlAttribute(AttributeName = "CheckNum")]
		public int CheckNum { get; set; }

		[XmlAttribute(AttributeName = "Id")]
		public string Id { get; set; }

		[XmlAttribute(AttributeName = "HarmonyId")]
		public int HarmonyId { get; set; }

		[XmlAttribute(AttributeName = "Timestamp")]
		public DateTime Timestamp { get; set; }
	}

	[XmlRoot(ElementName = "OraPayloadEntity")]
	public class OraPayloadEntityModel
	{

		[XmlElement(ElementName = "Check")]
		public CheckModel Check { get; set; }

		[XmlAttribute(AttributeName = "version")]
		public string Version { get; set; }
	}




}
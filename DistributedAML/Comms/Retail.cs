// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: Retail.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from Retail.proto</summary>
public static partial class RetailReflection {

  #region Descriptor
  /// <summary>File descriptor for Retail.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static RetailReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "CgxSZXRhaWwucHJvdG8i6wEKBlJldGFpbBIKCgJJZBgBIAEoCRIMCgROYW1l",
          "GAIgASgJEhMKC0NvbXBhbnlOYW1lGAMgASgJEhAKCEFkZHJlc3MxGAQgASgJ",
          "EhAKCEFkZHJlc3MyGAUgASgJEgwKBFRvd24YBiABKAkSDwoHQ291bnRyeRgH",
          "IAEoCRIQCghQb3N0Q29kZRgIIAEoCRISCgpUZWxlcGhvbmUxGAkgASgJEhIK",
          "ClRlbGVwaG9uZTIYCiABKAkSDQoFRW1haWwYCyABKAkSEgoKV2ViQWRkcmVz",
          "cxgNIAEoCRISCgpUeG5Qcm9maWxlGA4gASgJYgZwcm90bzM="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::Retail), global::Retail.Parser, new[]{ "Id", "Name", "CompanyName", "Address1", "Address2", "Town", "Country", "PostCode", "Telephone1", "Telephone2", "Email", "WebAddress", "TxnProfile" }, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class Retail : pb::IMessage<Retail> {
  private static readonly pb::MessageParser<Retail> _parser = new pb::MessageParser<Retail>(() => new Retail());
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<Retail> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::RetailReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Retail() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Retail(Retail other) : this() {
    id_ = other.id_;
    name_ = other.name_;
    companyName_ = other.companyName_;
    address1_ = other.address1_;
    address2_ = other.address2_;
    town_ = other.town_;
    country_ = other.country_;
    postCode_ = other.postCode_;
    telephone1_ = other.telephone1_;
    telephone2_ = other.telephone2_;
    email_ = other.email_;
    webAddress_ = other.webAddress_;
    txnProfile_ = other.txnProfile_;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Retail Clone() {
    return new Retail(this);
  }

  /// <summary>Field number for the "Id" field.</summary>
  public const int IdFieldNumber = 1;
  private string id_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Id {
    get { return id_; }
    set {
      id_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "Name" field.</summary>
  public const int NameFieldNumber = 2;
  private string name_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Name {
    get { return name_; }
    set {
      name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "CompanyName" field.</summary>
  public const int CompanyNameFieldNumber = 3;
  private string companyName_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string CompanyName {
    get { return companyName_; }
    set {
      companyName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "Address1" field.</summary>
  public const int Address1FieldNumber = 4;
  private string address1_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Address1 {
    get { return address1_; }
    set {
      address1_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "Address2" field.</summary>
  public const int Address2FieldNumber = 5;
  private string address2_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Address2 {
    get { return address2_; }
    set {
      address2_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "Town" field.</summary>
  public const int TownFieldNumber = 6;
  private string town_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Town {
    get { return town_; }
    set {
      town_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "Country" field.</summary>
  public const int CountryFieldNumber = 7;
  private string country_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Country {
    get { return country_; }
    set {
      country_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "PostCode" field.</summary>
  public const int PostCodeFieldNumber = 8;
  private string postCode_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string PostCode {
    get { return postCode_; }
    set {
      postCode_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "Telephone1" field.</summary>
  public const int Telephone1FieldNumber = 9;
  private string telephone1_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Telephone1 {
    get { return telephone1_; }
    set {
      telephone1_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "Telephone2" field.</summary>
  public const int Telephone2FieldNumber = 10;
  private string telephone2_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Telephone2 {
    get { return telephone2_; }
    set {
      telephone2_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "Email" field.</summary>
  public const int EmailFieldNumber = 11;
  private string email_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Email {
    get { return email_; }
    set {
      email_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "WebAddress" field.</summary>
  public const int WebAddressFieldNumber = 13;
  private string webAddress_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string WebAddress {
    get { return webAddress_; }
    set {
      webAddress_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "TxnProfile" field.</summary>
  public const int TxnProfileFieldNumber = 14;
  private string txnProfile_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string TxnProfile {
    get { return txnProfile_; }
    set {
      txnProfile_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as Retail);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(Retail other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Id != other.Id) return false;
    if (Name != other.Name) return false;
    if (CompanyName != other.CompanyName) return false;
    if (Address1 != other.Address1) return false;
    if (Address2 != other.Address2) return false;
    if (Town != other.Town) return false;
    if (Country != other.Country) return false;
    if (PostCode != other.PostCode) return false;
    if (Telephone1 != other.Telephone1) return false;
    if (Telephone2 != other.Telephone2) return false;
    if (Email != other.Email) return false;
    if (WebAddress != other.WebAddress) return false;
    if (TxnProfile != other.TxnProfile) return false;
    return true;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Id.Length != 0) hash ^= Id.GetHashCode();
    if (Name.Length != 0) hash ^= Name.GetHashCode();
    if (CompanyName.Length != 0) hash ^= CompanyName.GetHashCode();
    if (Address1.Length != 0) hash ^= Address1.GetHashCode();
    if (Address2.Length != 0) hash ^= Address2.GetHashCode();
    if (Town.Length != 0) hash ^= Town.GetHashCode();
    if (Country.Length != 0) hash ^= Country.GetHashCode();
    if (PostCode.Length != 0) hash ^= PostCode.GetHashCode();
    if (Telephone1.Length != 0) hash ^= Telephone1.GetHashCode();
    if (Telephone2.Length != 0) hash ^= Telephone2.GetHashCode();
    if (Email.Length != 0) hash ^= Email.GetHashCode();
    if (WebAddress.Length != 0) hash ^= WebAddress.GetHashCode();
    if (TxnProfile.Length != 0) hash ^= TxnProfile.GetHashCode();
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (Id.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Id);
    }
    if (Name.Length != 0) {
      output.WriteRawTag(18);
      output.WriteString(Name);
    }
    if (CompanyName.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(CompanyName);
    }
    if (Address1.Length != 0) {
      output.WriteRawTag(34);
      output.WriteString(Address1);
    }
    if (Address2.Length != 0) {
      output.WriteRawTag(42);
      output.WriteString(Address2);
    }
    if (Town.Length != 0) {
      output.WriteRawTag(50);
      output.WriteString(Town);
    }
    if (Country.Length != 0) {
      output.WriteRawTag(58);
      output.WriteString(Country);
    }
    if (PostCode.Length != 0) {
      output.WriteRawTag(66);
      output.WriteString(PostCode);
    }
    if (Telephone1.Length != 0) {
      output.WriteRawTag(74);
      output.WriteString(Telephone1);
    }
    if (Telephone2.Length != 0) {
      output.WriteRawTag(82);
      output.WriteString(Telephone2);
    }
    if (Email.Length != 0) {
      output.WriteRawTag(90);
      output.WriteString(Email);
    }
    if (WebAddress.Length != 0) {
      output.WriteRawTag(106);
      output.WriteString(WebAddress);
    }
    if (TxnProfile.Length != 0) {
      output.WriteRawTag(114);
      output.WriteString(TxnProfile);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Id.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Id);
    }
    if (Name.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
    }
    if (CompanyName.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(CompanyName);
    }
    if (Address1.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Address1);
    }
    if (Address2.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Address2);
    }
    if (Town.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Town);
    }
    if (Country.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Country);
    }
    if (PostCode.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(PostCode);
    }
    if (Telephone1.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Telephone1);
    }
    if (Telephone2.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Telephone2);
    }
    if (Email.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Email);
    }
    if (WebAddress.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(WebAddress);
    }
    if (TxnProfile.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(TxnProfile);
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(Retail other) {
    if (other == null) {
      return;
    }
    if (other.Id.Length != 0) {
      Id = other.Id;
    }
    if (other.Name.Length != 0) {
      Name = other.Name;
    }
    if (other.CompanyName.Length != 0) {
      CompanyName = other.CompanyName;
    }
    if (other.Address1.Length != 0) {
      Address1 = other.Address1;
    }
    if (other.Address2.Length != 0) {
      Address2 = other.Address2;
    }
    if (other.Town.Length != 0) {
      Town = other.Town;
    }
    if (other.Country.Length != 0) {
      Country = other.Country;
    }
    if (other.PostCode.Length != 0) {
      PostCode = other.PostCode;
    }
    if (other.Telephone1.Length != 0) {
      Telephone1 = other.Telephone1;
    }
    if (other.Telephone2.Length != 0) {
      Telephone2 = other.Telephone2;
    }
    if (other.Email.Length != 0) {
      Email = other.Email;
    }
    if (other.WebAddress.Length != 0) {
      WebAddress = other.WebAddress;
    }
    if (other.TxnProfile.Length != 0) {
      TxnProfile = other.TxnProfile;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          input.SkipLastField();
          break;
        case 10: {
          Id = input.ReadString();
          break;
        }
        case 18: {
          Name = input.ReadString();
          break;
        }
        case 26: {
          CompanyName = input.ReadString();
          break;
        }
        case 34: {
          Address1 = input.ReadString();
          break;
        }
        case 42: {
          Address2 = input.ReadString();
          break;
        }
        case 50: {
          Town = input.ReadString();
          break;
        }
        case 58: {
          Country = input.ReadString();
          break;
        }
        case 66: {
          PostCode = input.ReadString();
          break;
        }
        case 74: {
          Telephone1 = input.ReadString();
          break;
        }
        case 82: {
          Telephone2 = input.ReadString();
          break;
        }
        case 90: {
          Email = input.ReadString();
          break;
        }
        case 106: {
          WebAddress = input.ReadString();
          break;
        }
        case 114: {
          TxnProfile = input.ReadString();
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code

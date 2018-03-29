using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

/// <summary>
/// Value info for an affix type with multiple values
/// </summary>
[DataContract]
public class AffixValueInfoMultiple : AffixValueInfo
{
    [DataMember]
    public AffixValueInfo[] ValueInfos { get; private set; }

    public AffixValueInfoMultiple(List<AffixValueInfo> infos)
    {
        ValueInfos = new AffixValueInfo[infos.Count];
        infos.CopyTo(ValueInfos);
    }
}

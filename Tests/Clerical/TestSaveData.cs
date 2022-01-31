using System;

using FowlFever.BSharp.Clerical.Saving;

using Newtonsoft.Json;

namespace BSharp.Tests.Clerical; 

[JsonObject]
public class TestSaveData : SaveData {
    [JsonProperty]
    public Guid Guid { get; private set; } = Guid.NewGuid();

    [JsonProperty]
    public string Property_Public_Get { get; private set; }

    [JsonProperty]
    public string Field_Public;

    [JsonProperty]
    private int _counter;

    // [JsonIgnore]
    public int Counter {
        get => _counter;
        set => _counter = value;
    }

    // [JsonConstructor]
    public TestSaveData(string nickname) {
        Property_Public_Get = $"{nickname}_{nameof(Property_Public_Get)}";
        Field_Public        = $"{nickname}_{nameof(Field_Public)}";
    }

    public override string ToString() {
        return JsonConvert.SerializeObject(this);
    }

    protected bool Equals(TestSaveData other) {
        return Field_Public == other.Field_Public && _counter == other._counter && Guid.Equals(other.Guid) && Property_Public_Get == other.Property_Public_Get;
    }

    public override bool Equals(object obj) {
        if (ReferenceEquals(null, obj)) {
            return false;
        }

        if (ReferenceEquals(this, obj)) {
            return true;
        }

        if (obj.GetType() != this.GetType()) {
            return false;
        }

        return Equals((TestSaveData)obj);
    }

    public override int GetHashCode() {
        unchecked {
            var hashCode = (Field_Public != null ? Field_Public.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ _counter;
            hashCode = (hashCode * 397) ^ Guid.GetHashCode();
            hashCode = (hashCode * 397) ^ (Property_Public_Get != null ? Property_Public_Get.GetHashCode() : 0);
            return hashCode;
        }
    }
}
using System;
using Kernys.Bson;

namespace BasicTypes
{
	// Token: 0x02000311 RID: 785
	[Serializable]
	public struct Vector2i
	{
		// Token: 0x06001FDC RID: 8156 RVA: 0x000CF4E7 File Offset: 0x000CD8E7
		public Vector2i(int xx, int yy)
		{
			this.x = xx;
			this.y = yy;
		}

		// Token: 0x06001FDD RID: 8157 RVA: 0x000CF4F7 File Offset: 0x000CD8F7
		public Vector2i(BSONObject bson)
		{
			this.x = bson[Vector2i.xKey].int32Value;
			this.y = bson[Vector2i.yKey].int32Value;
		}

		// Token: 0x06001FDE RID: 8158 RVA: 0x000CF525 File Offset: 0x000CD925
		public Vector2i(byte[] byteArray)
		{
			this.x = BitConverter.ToInt32(byteArray, 0);
			this.y = BitConverter.ToInt32(byteArray, 4);
		}

		// Token: 0x06001FDF RID: 8159 RVA: 0x000CF541 File Offset: 0x000CD941
		public void StoreToBSON(BSONObject bson)
		{
			bson[Vector2i.xKey] = this.x;
			bson[Vector2i.yKey] = this.y;
		}

		// Token: 0x06001FE0 RID: 8160 RVA: 0x000CF570 File Offset: 0x000CD970
		public byte[] GetAsBinaryArray()
		{
			byte[] array = new byte[8];
			Buffer.BlockCopy(BitConverter.GetBytes(this.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(this.y), 0, array, 4, 4);
			return array;
		}

		// Token: 0x06001FE1 RID: 8161 RVA: 0x000CF5AD File Offset: 0x000CD9AD
		public static bool DoesValidate(BSONObject bson)
		{
			return bson.ContainsKey(Vector2i.xKey) && bson.ContainsKey(Vector2i.yKey);
		}

		// Token: 0x06001FE2 RID: 8162 RVA: 0x000CF5CD File Offset: 0x000CD9CD
		public override bool Equals(object obj)
		{
			return obj is Vector2i && this == (Vector2i)obj;
		}

		// Token: 0x06001FE3 RID: 8163 RVA: 0x000CF5EE File Offset: 0x000CD9EE
		public override int GetHashCode()
		{
			return this.x << 16 + this.y;
		}

		// Token: 0x06001FE4 RID: 8164 RVA: 0x000CF603 File Offset: 0x000CDA03
		public static bool operator ==(Vector2i v1, Vector2i v2)
		{
			return v1.x == v2.x && v1.y == v2.y;
		}

		// Token: 0x06001FE5 RID: 8165 RVA: 0x000CF62B File Offset: 0x000CDA2B
		public static bool operator !=(Vector2i v1, Vector2i v2)
		{
			return !(v1 == v2);
		}

		// Token: 0x06001FE6 RID: 8166 RVA: 0x000CF638 File Offset: 0x000CDA38
		public static Vector2i operator +(Vector2i v1, Vector2i v2)
		{
			Vector2i.tempAddition.x = v1.x;
			Vector2i.tempAddition.x = Vector2i.tempAddition.x + v2.x;
			Vector2i.tempAddition.y = v1.y;
			Vector2i.tempAddition.y = Vector2i.tempAddition.y + v2.y;
			return Vector2i.tempAddition;
		}

		// Token: 0x06001FE7 RID: 8167 RVA: 0x000CF69C File Offset: 0x000CDA9C
		public static Vector2i GetZero()
		{
			return new Vector2i(0, 0);
		}

		// Token: 0x06001FE8 RID: 8168 RVA: 0x000CF6A8 File Offset: 0x000CDAA8
		public static Vector2i GetFromString(string vec2iString)
		{
			string[] array = vec2iString.Split(new char[]
			{
				' '
			});
			return new Vector2i(int.Parse(array[0]), int.Parse(array[1]));
		}

		// Token: 0x06001FE9 RID: 8169 RVA: 0x000CF6DC File Offset: 0x000CDADC
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				Vector2i.xKey,
				" ",
				this.x,
				" ",
				Vector2i.yKey,
				" ",
				this.y
			});
		}

		// Token: 0x040023BC RID: 9148
		public int x;

		// Token: 0x040023BD RID: 9149
		public int y;

		// Token: 0x040023BE RID: 9150
		private static readonly string xKey = "x";

		// Token: 0x040023BF RID: 9151
		private static readonly string yKey = "y";

		// Token: 0x040023C0 RID: 9152
		public const int sizeInBytes = 8;

		// Token: 0x040023C1 RID: 9153
		private static Vector2i tempAddition = new Vector2i(0, 0);
	}
}

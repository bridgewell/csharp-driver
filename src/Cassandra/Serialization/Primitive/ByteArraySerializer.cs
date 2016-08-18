﻿//
//      Copyright (C) 2012-2016 DataStax Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

namespace BWCassandra.Serialization.Primitive
{
    internal class ByteArraySerializer : TypeSerializer<byte[]>
    {
        public override ColumnTypeCode CqlType
        {
            get { return ColumnTypeCode.Blob; }
        }

        public override byte[] Deserialize(ushort protocolVersion, byte[] buffer, int offset, int length, IColumnInfo typeInfo)
        {
            return Utils.FromOffset(buffer, offset, length);
        }

        public override byte[] Serialize(ushort protocolVersion, byte[] value)
        {
            return value;
        }
    }
}

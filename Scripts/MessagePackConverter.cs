using MessagePack;



namespace CloudStructures.Converters
{
    /// <summary>
    /// Provides value converter using MessagePack for C#.
    /// </summary>
    public sealed class MessagePackConverter : IValueConverter
    {
        public MessagePackConverter(bool compress = true) {
            Options = MessagePackSerializerOptions.Standard.WithCompression(
                compress ? MessagePackCompression.Lz4BlockArray : MessagePackCompression.None);
        }

        public MessagePackSerializerOptions Options { get; set; }

        /// <summary>
        /// Serialize value to binary.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Serialize<T>(T value)
            => MessagePackSerializer.Serialize(value, Options);


        /// <summary>
        /// Deserialize value from binary.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T Deserialize<T>(byte[] value)
            => MessagePackSerializer.Deserialize<T>(value, Options);
    }
}


﻿//Source: https://github.com/RobThree/IdGen
using System;
using System.Globalization;

namespace IdGen.Configuration
{
	/// <summary>
	/// Represents an IdGenerator configuration element. This class cannot be inherited.
	/// </summary>
	public sealed class IdGeneratorElement : ConfigurationElement
	{
		private readonly string[] DATETIMEFORMATS = { "yyyy-MM-dd\\THH:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd" };

		/// <summary>
		/// Gets/sets the name of the <see cref="IdGeneratorElement"/>.
		/// </summary>
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get => (string)this["name"];
			set => this["name"] = value;
		}

		/// <summary>
		/// Gets/sets the GeneratorId of the <see cref="IdGeneratorElement"/>.
		/// </summary>
		[ConfigurationProperty("id", IsRequired = true)]
		public int Id
		{
			get => (int)this["id"];
			set => this["id"] = value;
		}

		[ConfigurationProperty("epoch", IsRequired = true)]
		private string StringEpoch
		{
			get => (string)this["epoch"];
			set => this["epoch"] = value;
		}

		/// <summary>
		/// Gets/sets the <see cref="SequenceOverflowStrategy"/> option of the <see cref="IdGeneratorElement"/>.
		/// </summary>
		[ConfigurationProperty("sequenceOverflowStrategy", IsRequired = false)]
		public SequenceOverflowStrategy SequenceOverflowStrategy
		{
			get => (SequenceOverflowStrategy)this["sequenceOverflowStrategy"];
			set => this["sequenceOverflowStrategy"] = value;
		}

		/// <summary>
		/// Gets/sets the Epoch of the <see cref="IdGeneratorElement"/>.
		/// </summary>
		public DateTime Epoch
		{
			get => DateTime.SpecifyKind(DateTime.ParseExact(StringEpoch, DATETIMEFORMATS, CultureInfo.InvariantCulture, DateTimeStyles.None), DateTimeKind.Utc);
			set => StringEpoch = value.ToString(DATETIMEFORMATS[0], CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Gets/sets the <see cref="IdStructure.TimestampBits"/> of the <see cref="IdGeneratorElement"/>.
		/// </summary>
		[ConfigurationProperty("timestampBits", IsRequired = true)]
		public byte TimestampBits
		{
			get => (byte)this["timestampBits"];
			set => this["timestampBits"] = value;
		}

		/// <summary>
		/// Gets/sets the <see cref="IdStructure.GeneratorIdBits"/> of the <see cref="IdGeneratorElement"/>.
		/// </summary>
		[ConfigurationProperty("generatorIdBits", IsRequired = true)]
		public byte GeneratorIdBits
		{
			get => (byte)this["generatorIdBits"];
			set => this["generatorIdBits"] = value;
		}

		/// <summary>
		/// Gets/sets the <see cref="IdStructure.SequenceBits"/> of the <see cref="IdGeneratorElement"/>.
		/// </summary>
		[ConfigurationProperty("sequenceBits", IsRequired = true)]
		public byte SequenceBits
		{
			get => (byte)this["sequenceBits"];
			set => this["sequenceBits"] = value;
		}

		/// <summary>
		/// Gets/sets the <see cref="ITimeSource.TickDuration"/> of the <see cref="IdGeneratorElement"/>.
		/// </summary>
		[ConfigurationProperty("tickDuration", IsRequired = false)]
		public TimeSpan TickDuration
		{
			get => (TimeSpan)this["tickDuration"];
			set => this["tickDuration"] = value;
		}
	}
}
﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace cabme.data
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="content")]
	public partial class contentDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertBooking(Booking instance);
    partial void UpdateBooking(Booking instance);
    partial void DeleteBooking(Booking instance);
    partial void InsertTaxi(Taxi instance);
    partial void UpdateTaxi(Taxi instance);
    partial void DeleteTaxi(Taxi instance);
    #endregion
		
		public contentDataContext() : 
				base(global::cabme.data.Properties.Settings.Default.contentConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public contentDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public contentDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public contentDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public contentDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Booking> Bookings
		{
			get
			{
				return this.GetTable<Booking>();
			}
		}
		
		public System.Data.Linq.Table<Taxi> Taxis
		{
			get
			{
				return this.GetTable<Taxi>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Booking")]
	public partial class Booking : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Name;
		
		private string _PhoneNumber;
		
		private short _NumberOfPeople;
		
		private System.DateTime _PickupTime;
		
		private string _AddrFrom;
		
		private System.Nullable<int> _LatitudeFrom;
		
		private System.Nullable<int> _LongitudeFrom;
		
		private string _AddrTo;
		
		private System.Nullable<int> _LatitudeTo;
		
		private System.Nullable<int> _LongitudeTo;
		
		private int _ComputedDistance;
		
		private int _EstimatedPrice;
		
		private bool _Active;
		
		private bool _Confirmed;
		
		private System.Nullable<int> _TaxiId;
		
		private System.DateTime _LastModified;
		
		private System.DateTime _Created;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnPhoneNumberChanging(string value);
    partial void OnPhoneNumberChanged();
    partial void OnNumberOfPeopleChanging(short value);
    partial void OnNumberOfPeopleChanged();
    partial void OnPickupTimeChanging(System.DateTime value);
    partial void OnPickupTimeChanged();
    partial void OnAddrFromChanging(string value);
    partial void OnAddrFromChanged();
    partial void OnLatitudeFromChanging(System.Nullable<int> value);
    partial void OnLatitudeFromChanged();
    partial void OnLongitudeFromChanging(System.Nullable<int> value);
    partial void OnLongitudeFromChanged();
    partial void OnAddrToChanging(string value);
    partial void OnAddrToChanged();
    partial void OnLatitudeToChanging(System.Nullable<int> value);
    partial void OnLatitudeToChanged();
    partial void OnLongitudeToChanging(System.Nullable<int> value);
    partial void OnLongitudeToChanged();
    partial void OnComputedDistanceChanging(int value);
    partial void OnComputedDistanceChanged();
    partial void OnEstimatedPriceChanging(int value);
    partial void OnEstimatedPriceChanged();
    partial void OnActiveChanging(bool value);
    partial void OnActiveChanged();
    partial void OnConfirmedChanging(bool value);
    partial void OnConfirmedChanged();
    partial void OnTaxiIdChanging(System.Nullable<int> value);
    partial void OnTaxiIdChanged();
    partial void OnLastModifiedChanging(System.DateTime value);
    partial void OnLastModifiedChanged();
    partial void OnCreatedChanging(System.DateTime value);
    partial void OnCreatedChanged();
    #endregion
		
		public Booking()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="VarChar(60)")]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PhoneNumber", DbType="VarChar(20) NOT NULL", CanBeNull=false)]
		public string PhoneNumber
		{
			get
			{
				return this._PhoneNumber;
			}
			set
			{
				if ((this._PhoneNumber != value))
				{
					this.OnPhoneNumberChanging(value);
					this.SendPropertyChanging();
					this._PhoneNumber = value;
					this.SendPropertyChanged("PhoneNumber");
					this.OnPhoneNumberChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NumberOfPeople", DbType="SmallInt NOT NULL")]
		public short NumberOfPeople
		{
			get
			{
				return this._NumberOfPeople;
			}
			set
			{
				if ((this._NumberOfPeople != value))
				{
					this.OnNumberOfPeopleChanging(value);
					this.SendPropertyChanging();
					this._NumberOfPeople = value;
					this.SendPropertyChanged("NumberOfPeople");
					this.OnNumberOfPeopleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PickupTime", DbType="DateTime NOT NULL")]
		public System.DateTime PickupTime
		{
			get
			{
				return this._PickupTime;
			}
			set
			{
				if ((this._PickupTime != value))
				{
					this.OnPickupTimeChanging(value);
					this.SendPropertyChanging();
					this._PickupTime = value;
					this.SendPropertyChanged("PickupTime");
					this.OnPickupTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AddrFrom", DbType="VarChar(400) NOT NULL", CanBeNull=false)]
		public string AddrFrom
		{
			get
			{
				return this._AddrFrom;
			}
			set
			{
				if ((this._AddrFrom != value))
				{
					this.OnAddrFromChanging(value);
					this.SendPropertyChanging();
					this._AddrFrom = value;
					this.SendPropertyChanged("AddrFrom");
					this.OnAddrFromChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LatitudeFrom", DbType="Int")]
		public System.Nullable<int> LatitudeFrom
		{
			get
			{
				return this._LatitudeFrom;
			}
			set
			{
				if ((this._LatitudeFrom != value))
				{
					this.OnLatitudeFromChanging(value);
					this.SendPropertyChanging();
					this._LatitudeFrom = value;
					this.SendPropertyChanged("LatitudeFrom");
					this.OnLatitudeFromChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LongitudeFrom", DbType="Int")]
		public System.Nullable<int> LongitudeFrom
		{
			get
			{
				return this._LongitudeFrom;
			}
			set
			{
				if ((this._LongitudeFrom != value))
				{
					this.OnLongitudeFromChanging(value);
					this.SendPropertyChanging();
					this._LongitudeFrom = value;
					this.SendPropertyChanged("LongitudeFrom");
					this.OnLongitudeFromChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AddrTo", DbType="VarChar(400)")]
		public string AddrTo
		{
			get
			{
				return this._AddrTo;
			}
			set
			{
				if ((this._AddrTo != value))
				{
					this.OnAddrToChanging(value);
					this.SendPropertyChanging();
					this._AddrTo = value;
					this.SendPropertyChanged("AddrTo");
					this.OnAddrToChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LatitudeTo", DbType="Int")]
		public System.Nullable<int> LatitudeTo
		{
			get
			{
				return this._LatitudeTo;
			}
			set
			{
				if ((this._LatitudeTo != value))
				{
					this.OnLatitudeToChanging(value);
					this.SendPropertyChanging();
					this._LatitudeTo = value;
					this.SendPropertyChanged("LatitudeTo");
					this.OnLatitudeToChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LongitudeTo", DbType="Int")]
		public System.Nullable<int> LongitudeTo
		{
			get
			{
				return this._LongitudeTo;
			}
			set
			{
				if ((this._LongitudeTo != value))
				{
					this.OnLongitudeToChanging(value);
					this.SendPropertyChanging();
					this._LongitudeTo = value;
					this.SendPropertyChanged("LongitudeTo");
					this.OnLongitudeToChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ComputedDistance", DbType="Int NOT NULL")]
		public int ComputedDistance
		{
			get
			{
				return this._ComputedDistance;
			}
			set
			{
				if ((this._ComputedDistance != value))
				{
					this.OnComputedDistanceChanging(value);
					this.SendPropertyChanging();
					this._ComputedDistance = value;
					this.SendPropertyChanged("ComputedDistance");
					this.OnComputedDistanceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EstimatedPrice", DbType="Int NOT NULL")]
		public int EstimatedPrice
		{
			get
			{
				return this._EstimatedPrice;
			}
			set
			{
				if ((this._EstimatedPrice != value))
				{
					this.OnEstimatedPriceChanging(value);
					this.SendPropertyChanging();
					this._EstimatedPrice = value;
					this.SendPropertyChanged("EstimatedPrice");
					this.OnEstimatedPriceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Active", DbType="Bit NOT NULL")]
		public bool Active
		{
			get
			{
				return this._Active;
			}
			set
			{
				if ((this._Active != value))
				{
					this.OnActiveChanging(value);
					this.SendPropertyChanging();
					this._Active = value;
					this.SendPropertyChanged("Active");
					this.OnActiveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Confirmed", DbType="Bit NOT NULL")]
		public bool Confirmed
		{
			get
			{
				return this._Confirmed;
			}
			set
			{
				if ((this._Confirmed != value))
				{
					this.OnConfirmedChanging(value);
					this.SendPropertyChanging();
					this._Confirmed = value;
					this.SendPropertyChanged("Confirmed");
					this.OnConfirmedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TaxiId", DbType="Int")]
		public System.Nullable<int> TaxiId
		{
			get
			{
				return this._TaxiId;
			}
			set
			{
				if ((this._TaxiId != value))
				{
					this.OnTaxiIdChanging(value);
					this.SendPropertyChanging();
					this._TaxiId = value;
					this.SendPropertyChanged("TaxiId");
					this.OnTaxiIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastModified", DbType="DateTime NOT NULL")]
		public System.DateTime LastModified
		{
			get
			{
				return this._LastModified;
			}
			set
			{
				if ((this._LastModified != value))
				{
					this.OnLastModifiedChanging(value);
					this.SendPropertyChanging();
					this._LastModified = value;
					this.SendPropertyChanged("LastModified");
					this.OnLastModifiedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Created", DbType="DateTime NOT NULL")]
		public System.DateTime Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				if ((this._Created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Taxi")]
	public partial class Taxi : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Name;
		
		private string _PhoneNumber;
		
		private int _RatePerKm;
		
		private int _MinRate;
		
		private short _Units;
		
		private System.DateTime _StartOfService;
		
		private System.DateTime _EndOfService;
		
		private bool _Is24HService;
		
		private short _FleetSize;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnPhoneNumberChanging(string value);
    partial void OnPhoneNumberChanged();
    partial void OnRatePerKmChanging(int value);
    partial void OnRatePerKmChanged();
    partial void OnMinRateChanging(int value);
    partial void OnMinRateChanged();
    partial void OnUnitsChanging(short value);
    partial void OnUnitsChanged();
    partial void OnStartOfServiceChanging(System.DateTime value);
    partial void OnStartOfServiceChanged();
    partial void OnEndOfServiceChanging(System.DateTime value);
    partial void OnEndOfServiceChanged();
    partial void OnIs24HServiceChanging(bool value);
    partial void OnIs24HServiceChanged();
    partial void OnFleetSizeChanging(short value);
    partial void OnFleetSizeChanged();
    #endregion
		
		public Taxi()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="VarChar(60) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PhoneNumber", DbType="VarChar(20) NOT NULL", CanBeNull=false)]
		public string PhoneNumber
		{
			get
			{
				return this._PhoneNumber;
			}
			set
			{
				if ((this._PhoneNumber != value))
				{
					this.OnPhoneNumberChanging(value);
					this.SendPropertyChanging();
					this._PhoneNumber = value;
					this.SendPropertyChanged("PhoneNumber");
					this.OnPhoneNumberChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RatePerKm", DbType="Int NOT NULL")]
		public int RatePerKm
		{
			get
			{
				return this._RatePerKm;
			}
			set
			{
				if ((this._RatePerKm != value))
				{
					this.OnRatePerKmChanging(value);
					this.SendPropertyChanging();
					this._RatePerKm = value;
					this.SendPropertyChanged("RatePerKm");
					this.OnRatePerKmChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MinRate", DbType="Int NOT NULL")]
		public int MinRate
		{
			get
			{
				return this._MinRate;
			}
			set
			{
				if ((this._MinRate != value))
				{
					this.OnMinRateChanging(value);
					this.SendPropertyChanging();
					this._MinRate = value;
					this.SendPropertyChanged("MinRate");
					this.OnMinRateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Units", DbType="SmallInt NOT NULL")]
		public short Units
		{
			get
			{
				return this._Units;
			}
			set
			{
				if ((this._Units != value))
				{
					this.OnUnitsChanging(value);
					this.SendPropertyChanging();
					this._Units = value;
					this.SendPropertyChanged("Units");
					this.OnUnitsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_StartOfService", DbType="DateTime NOT NULL")]
		public System.DateTime StartOfService
		{
			get
			{
				return this._StartOfService;
			}
			set
			{
				if ((this._StartOfService != value))
				{
					this.OnStartOfServiceChanging(value);
					this.SendPropertyChanging();
					this._StartOfService = value;
					this.SendPropertyChanged("StartOfService");
					this.OnStartOfServiceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EndOfService", DbType="DateTime NOT NULL")]
		public System.DateTime EndOfService
		{
			get
			{
				return this._EndOfService;
			}
			set
			{
				if ((this._EndOfService != value))
				{
					this.OnEndOfServiceChanging(value);
					this.SendPropertyChanging();
					this._EndOfService = value;
					this.SendPropertyChanged("EndOfService");
					this.OnEndOfServiceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Is24HService", DbType="Bit NOT NULL")]
		public bool Is24HService
		{
			get
			{
				return this._Is24HService;
			}
			set
			{
				if ((this._Is24HService != value))
				{
					this.OnIs24HServiceChanging(value);
					this.SendPropertyChanging();
					this._Is24HService = value;
					this.SendPropertyChanged("Is24HService");
					this.OnIs24HServiceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FleetSize", DbType="SmallInt NOT NULL")]
		public short FleetSize
		{
			get
			{
				return this._FleetSize;
			}
			set
			{
				if ((this._FleetSize != value))
				{
					this.OnFleetSizeChanging(value);
					this.SendPropertyChanging();
					this._FleetSize = value;
					this.SendPropertyChanged("FleetSize");
					this.OnFleetSizeChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591

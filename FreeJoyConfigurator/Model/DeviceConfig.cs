﻿using HidLibrary;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace FreeJoyConfigurator
{

    public class AxisConfig : BindableBase
    {
        private ushort _calibMin;
        private ushort _calibCenter;
        private ushort _calibMax;
        private bool _isInverted;
        private bool _isMagnetOffset;
        private ObservableCollection<Point> _curveShape;
        private byte _filterLevel;
        private bool _isOutEnabled;
        private byte _resolution;
        

        private bool _isCalibCenterUnlocked;

        public ushort CalibMin
        {
            get
            {
                return _calibMin;
            }
            set
            {
                if (value < 0) SetProperty(ref _calibMin, (ushort)0);
                else if (value >= CalibCenter && IsCalibCenterUnlocked) SetProperty(ref _calibMin, (ushort)(CalibCenter - 1));
                else if (value >= CalibMax) SetProperty(ref _calibMin, (ushort)(CalibMax - 2));
                else SetProperty(ref _calibMin, value);

                if (!IsCalibCenterUnlocked)
                {
                    CalibCenter = (ushort)((CalibMax - CalibMin) / 2 + CalibMin);
                }
            }
        }
        public ushort CalibCenter
        {
            get
            {
                return _calibCenter;
            }
            set
            {
                if (IsCalibCenterUnlocked)
                {
                    if (value <= CalibMin)
                    {
                        SetProperty(ref _calibCenter, (ushort)(CalibMin + 1));
                    }
                    else if (value >= CalibMax)
                    {
                        SetProperty(ref _calibCenter, (ushort)(CalibMax - 1));
                    }
                    else SetProperty(ref _calibCenter, value);
                }
                else
                {
                    SetProperty(ref _calibCenter, (ushort)((CalibMax - CalibMin) / 2 + CalibMin));
                }
            }
        }
        public ushort CalibMax
        {
            get
            {
                return _calibMax;
            }
            set
            {
                if (value <= CalibMin) SetProperty(ref _calibMax, (ushort)(CalibMin + 2));
                else if (value <= CalibCenter && IsCalibCenterUnlocked) SetProperty(ref _calibMax, (ushort)(CalibCenter + 1));
                else if (value > 4095) SetProperty(ref _calibMax, (ushort) 4095);
                else SetProperty(ref _calibMax, value);

                if (!IsCalibCenterUnlocked)
                {
                    CalibCenter = (ushort)((CalibMax - CalibMin) / 2 + CalibMin);
                }
            }
        }
      
        public bool IsInverted
        {
            get
            {
                return _isInverted;
            }
            set
            {
                SetProperty(ref _isInverted, value);
            }
        }

        public bool IsMagnetOffset
        {
            get
            {
                return _isMagnetOffset;
            }
            set
            {
                SetProperty(ref _isMagnetOffset, value);
            }
        }

        public ObservableCollection<Point> CurveShape
        {
            get
            {
                return _curveShape;
            }
            set
            {
                SetProperty(ref _curveShape, value);
            }
        }

        public byte FilterLevel
        {
            get
            {
                return _filterLevel;
            }
            set
            {
                SetProperty(ref _filterLevel, value);
            }
        }

        public bool IsOutEnabled
        {
            get
            {
                return _isOutEnabled;
            }
            set
            {
                SetProperty(ref _isOutEnabled, value);
            }
        }

        public byte Resolution
        {
            get
            {
                return _resolution;
            }
            set
            {
                SetProperty(ref _resolution, value);
            }
        }

        public bool IsCalibCenterUnlocked
        {
            get
            {
                return _isCalibCenterUnlocked;
            }
            set
            {
                SetProperty(ref _isCalibCenterUnlocked, value);
                if (!_isCalibCenterUnlocked)
                {
                    CalibCenter = (ushort)((CalibMax - CalibMin) / 2);
                }
            }
        }

        public AxisConfig()
        {
            _calibMin = 0;
            _calibCenter = 2047;
            _calibMax = 4095;
            _isInverted = false;
            _isMagnetOffset = false;
            _isOutEnabled = true;
            _resolution = 12;

            _curveShape = new ObservableCollection<Point>();
            for (int i = 0; i < 10; i++) _curveShape.Add(new Point(i, 0));
            _filterLevel = 0;

            _isCalibCenterUnlocked = false;
        }

    }

    public class FilterLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string converted;

            Byte original = (Byte) value;

            if (original == 0) converted = "Off";
            else if(original == 1) converted = "Low";
            else if (original == 2) converted = "Medium";
            else if(original == 3) converted = "High";
            else converted = "Filter No";

            return converted;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public enum PinType : byte
    {
        NotUsed = 0,
        ButtonGnd,
        ButtonVcc,
        ButtonRow,
        ButtonColumn,

        AxisAnalog,
//        AxisToButtons,

        SPI_SCK = 7,

        TLE501x_CS,
        TLE501x_DATA,
        TLE501x_GEN,

        ShiftReg_LATCH,
        ShiftReg_DATA,
        
    };


    public enum ButtonType
    {
        BtnNormal = 0,
        BtnInverted,
        BtnToggle,
        ToggleSw,
        ToggleSwOn,
        ToggleSwOff,

        Pov1Up,
        Pov1Right,
        Pov1Down,
        Pov1Left,
        Pov2Up,
        Pov2Right,
        Pov2Down,
        Pov2Left,
        Pov3Up,
        Pov3Right,
        Pov3Down,
        Pov3Left,
        Pov4Up,
        Pov4Right,
        Pov4Down,
        Pov4Left,

        Encoder_A,
        Encoder_B,
        
        
    };

    public class ButtonConfig : BindableBase
    {
        private ButtonType _type;
        public ButtonType Type
        {
            get
            {
                return _type;
            }
            set
            {
                SetProperty(ref _type, value);
            }
        }

        public ButtonConfig()
        {
            _type = ButtonType.BtnNormal;
        }

        public ButtonConfig (ButtonType type)
        {
            _type = type;
        }
    }

    public class AxisToButtonsConfig : BindableBase
    {
        private ObservableCollection<sbyte> _points;
        private byte _buttonsCnt;
        private bool _isEnabled;

        public ObservableCollection<sbyte> Points
        {
            get { return _points; }
            set { SetProperty(ref _points, value); }
        }

        public byte ButtonsCnt
        {
            get { return _buttonsCnt; }
            set { SetProperty(ref _buttonsCnt, value); }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public AxisToButtonsConfig()
        {
            _points = new ObservableCollection<sbyte>();
            for (int i = 0; i < 13; i++) _points.Add(new sbyte());

            _points[0] = 0;
            _points[1] = 50;
            _points[2] = 100;

            _buttonsCnt = 2;
            _isEnabled = false;
        }
    }

    public enum ShiftRegisterType
    {
        HC165 = 0,
        CD4021 = 1,
    };

    public class ShiftRegisterConfig : BindableBase
    {
        private ShiftRegisterType _type;
        private short _buttonCnt;
        //private byte _selectPin;
        //private byte _dataPin;

        public ShiftRegisterType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        public short ButtonCnt
        {
            get { return _buttonCnt; }
            set { SetProperty(ref _buttonCnt, value); }
        }

        //public byte LatchPin
        //{
        //    get { return _selectPin; }
        //    set { SetProperty(ref _selectPin, value); }
        //}
        //public byte ClockPin

        //{
        //    get { return _dataPin; }
        //    set { SetProperty(ref _dataPin, value); }
        //}

        public ShiftRegisterConfig()
        {
            _type = ShiftRegisterType.CD4021;
            _buttonCnt = 0;
            //_selectPin = 0xFF;
            //_dataPin = 0xFF;
        }
    }

    public class DeviceConfig : BindableBase
    {
        [XmlElement("Firmware_Version")]
        public UInt16 FirmwareVersion { get; set; }
        [XmlElement("Device_Name")]
        public string DeviceName { get; set; }
        [XmlElement("Button_Debounce_Time")]
        public UInt16 ButtonDebounceMs { get; set; }
        [XmlElement("Toggle_Press_Time")]
        public UInt16 TogglePressMs { get; set; }
        [XmlElement("Encoder_Press_Time")]
        public UInt16 EncoderPressMs { get; set; }
        [XmlElement("Exchange_Period")]
        public UInt16 ExchangePeriod { get; set; }
        [XmlElement("Pin_Config")]
        public ObservableCollection<PinType> PinConfig { get; set; }
        [XmlElement("Axis_Config")]
        public ObservableCollection<AxisConfig> AxisConfig { get; set; }
        [XmlElement("Button_Config")]
        public ObservableCollection<ButtonConfig> ButtonConfig { get; set; }
        [XmlElement("AxisToButtons_Config")]
        public ObservableCollection<AxisToButtonsConfig> AxisToButtonsConfig { get; set; }
        [XmlElement("ShitRegisters_Config")]
        public ObservableCollection<ShiftRegisterConfig> ShiftRegistersConfig { get; set; }

        public DeviceConfig()
        {
            DeviceName = "FreeJoy";
            ButtonDebounceMs = 50;
            TogglePressMs = 300;
            EncoderPressMs = 100;
            ExchangePeriod = 10;

            AxisConfig = new ObservableCollection<AxisConfig>();
            for (int i = 0; i < 8; i++) AxisConfig.Add(new AxisConfig());

            PinConfig = new ObservableCollection<PinType>();
            for (int i = 0; i < 30; i++) PinConfig.Add(PinType.NotUsed);

            ButtonConfig = new ObservableCollection<ButtonConfig>();
            for (int i = 0; i < 128; i++) ButtonConfig.Add(new ButtonConfig());

            AxisToButtonsConfig = new ObservableCollection<AxisToButtonsConfig>();
            for (int i = 0; i < 8; i++) AxisToButtonsConfig.Add(new AxisToButtonsConfig());

            ShiftRegistersConfig = new ObservableCollection<ShiftRegisterConfig>();
            for (int i = 0; i < 4; i++) ShiftRegistersConfig.Add(new ShiftRegisterConfig());
        }
    }
        
}
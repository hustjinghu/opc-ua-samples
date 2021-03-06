// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Template10.Common;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Workstation.Collections;
using Workstation.ServiceModel.Ua;

namespace RobotApp.ViewModels
{
    /// <summary>
    /// A type of switch with three positions
    /// </summary>
    public enum HandOffAuto : short
    {
        Off = 0,
        Hand = 1,
        Auto = 2
    }

    /// <summary>
    /// A view model for MainPage.
    /// </summary>
    [Subscription(endpointUrl: "opc.tcp://localhost:26543", publishingInterval: 250, keepAliveCount: 20)]
    public class MainPageViewModel : SubscriptionBase, INavigable
    {
        /// <summary>
        /// Gets or sets the value of Robot1Mode.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Mode")]
        public short Robot1Mode
        {
            get { return this.robot1Mode; }
            set { this.SetProperty(ref this.robot1Mode, value); }
        }

        private short robot1Mode;

        /// <summary>
        /// Gets or sets the value of Robot1Axis1.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis1")]
        public float Robot1Axis1
        {
            get { return this.robot1Axis1; }
            set { this.SetProperty(ref this.robot1Axis1, value); }
        }

        private float robot1Axis1;

        /// <summary>
        /// Gets or sets the value of Robot1Axis2.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis2")]
        public float Robot1Axis2
        {
            get { return this.robot1Axis2; }
            set { this.SetProperty(ref this.robot1Axis2, value); }
        }

        private float robot1Axis2;

        /// <summary>
        /// Gets or sets the value of Robot1Axis3.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis3")]
        public float Robot1Axis3
        {
            get { return this.robot1Axis3; }
            set { this.SetProperty(ref this.robot1Axis3, value); }
        }

        private float robot1Axis3 = -45;

        /// <summary>
        /// Gets or sets the value of Robot1Axis4.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis4")]
        public float Robot1Axis4
        {
            get { return this.robot1Axis4; }
            set { this.SetProperty(ref this.robot1Axis4, value); }
        }

        private float robot1Axis4;

        /// <summary>
        /// Gets or sets the value of Robot1Speed.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Speed")]
        public short Robot1Speed
        {
            get { return this.robot1Speed; }
            set { this.SetProperty(ref this.robot1Speed, value); }
        }

        private short robot1Speed;

        /// <summary>
        /// Gets or sets a value indicating whether Robot1Laser is active.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Laser")]
        public bool Robot1Laser
        {
            get { return this.robot1Laser; }
            set { this.SetProperty(ref this.robot1Laser, value); }
        }

        private bool robot1Laser;

        /// <summary>
        /// Gets the recent history of Robot1Axis1.
        /// </summary>
        public ObservableQueue<DataValue> Robot1Axis1Queue { get; } = new ObservableQueue<DataValue>(capacity: 240, isFixedSize: true);

        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis1", dataChangeTrigger: DataChangeTrigger.StatusValueTimestamp)]
        private DataValue Robot1Axis1Stream { set { this.Robot1Axis1Queue.Enqueue(value); }}

        /// <summary>
        /// Gets the recent history of Robot1Axis2.
        /// </summary>
        public ObservableQueue<DataValue> Robot1Axis2Queue { get; } = new ObservableQueue<DataValue>(capacity: 240, isFixedSize: true);

        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis2", dataChangeTrigger: DataChangeTrigger.StatusValueTimestamp)]
        private DataValue Robot1Axis2Stream { set { this.Robot1Axis2Queue.Enqueue(value); }}

        /// <summary>
        /// Gets the recent history of Robot1Axis3.
        /// </summary>
        public ObservableQueue<DataValue> Robot1Axis3Queue { get; } = new ObservableQueue<DataValue>(capacity: 240, isFixedSize: true);

        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis3", dataChangeTrigger: DataChangeTrigger.StatusValueTimestamp)]
        private DataValue Robot1Axis3Stream { set { this.Robot1Axis3Queue.Enqueue(value); }}

        /// <summary>
        /// Gets the recent history of Robot1Axis4.
        /// </summary>
        public ObservableQueue<DataValue> Robot1Axis4Queue { get; } = new ObservableQueue<DataValue>(capacity: 240, isFixedSize: true);

        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis4", dataChangeTrigger: DataChangeTrigger.StatusValueTimestamp)]
        private DataValue Robot1Axis4Stream { set { this.Robot1Axis4Queue.Enqueue(value); }}

        /// <summary>
        /// Gets the current events of Robot1.
        /// </summary>
        /// <remarks>
        /// In this sample, we wish to display only recent events. So after 5 seconds the event is removed.
        /// This works nicely with the ItemsControl's AddDeleteThemeTransition.
        /// </remarks>
        public ObservableCollection<AlarmCondition> Robot1Events { get; } = new ObservableCollection<AlarmCondition>();

        [MonitoredItem(nodeId: "ns=2;s=Robot1", attributeId: AttributeIds.EventNotifier, queueSize: 8)]
        private AlarmCondition Robot1EventStream
        {
            set
            {
                this.Dispatcher.DispatchAsync(async () =>
                {
                    this.Robot1Events.Add(value);
                    await Task.Delay(5000);
                    this.Robot1Events.Remove(value);
                });
            }
        }

        /// <summary>
        /// Sets the Robot1Mode to Off.
        /// </summary>
        public async void Robot1ModeOff()
        {
            try
            {
                await this.InnerChannel.WriteAsync(new WriteRequest
                {
                    NodesToWrite = new[]
                    {
                        new WriteValue
                        {
                            NodeId = NodeId.Parse("ns=2;s=Robot1_Mode"),
                            AttributeId = AttributeIds.Value,
                            IndexRange = null,
                            Value = new DataValue((short)HandOffAuto.Off)
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error writing to NodeId {0} : {1}", "ns=2;s=Robot1_Mode", ex);
            }
        }

        /// <summary>
        /// Sets the Robot1Mode to Auto.
        /// </summary>
        public async void Robot1ModeAuto()
        {
            try
            {
                await this.InnerChannel.WriteAsync(new WriteRequest
                {
                    NodesToWrite = new[]
                    {
                        new WriteValue
                        {
                            NodeId = NodeId.Parse("ns=2;s=Robot1_Mode"),
                            AttributeId = AttributeIds.Value,
                            IndexRange = null,
                            Value = new DataValue((short)HandOffAuto.Auto)
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error writing to NodeId {0} : {1}", "ns=2;s=Robot1_Mode", ex);
            }
        }

        /// <summary>
        /// Gets or sets the value of InputA.
        /// </summary>
        public double InputA
        {
            get { return this.inputA; }
            set { this.SetProperty(ref this.inputA, value); }
        }

        private double inputA;

        /// <summary>
        /// Gets or sets the value of InputB.
        /// </summary>
        public double InputB
        {
            get { return this.inputB; }
            set { this.SetProperty(ref this.inputB, value); }
        }

        private double inputB;

        /// <summary>
        /// Gets or sets the value of Result.
        /// </summary>
        public double Result
        {
            get { return this.result; }
            set { this.SetProperty(ref this.result, value); }
        }

        private double result;

        /// <summary>
        /// Invokes the method Robot1Multiply.
        /// </summary>
        public async void Robot1Multiply()
        {
            try
            {
                // Call the method, passing the input arguments in a Variant[].
                var response = await this.InnerChannel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse("ns=2;s=Robot1"),
                            MethodId = NodeId.Parse("ns=2;s=Robot1_Multiply"),
                            InputArguments = new Variant[] { this.InputA, this.InputB }
                        }
                    }
                });

                // When the method returns, save the output argument.
                this.Result = (double)response.Results[0].OutputArguments[0];
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error calling Robot1Multiply method: {0}", ex);
            }
        }

        // Clear the form.
        public void Clear()
        {
            this.InputA = 0d;
            this.InputB = 0d;
            this.Result = 0d;
#if DEBUG
            GC.Collect();
#endif
        }

        public void GotoSettings() =>
            this.NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            this.NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            this.NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        // INavigable
        [JsonIgnore]
        public virtual INavigationService NavigationService { get; set; }

        [JsonIgnore]
        public virtual IDispatcherWrapper Dispatcher { get; set; }

        [JsonIgnore]
        public virtual IStateItems SessionState { get; set; }

        public virtual Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
namespace HaiMessenger

open Xamarin.Forms
open uPLibrary.Networking.M2Mqtt
open uPLibrary.Networking.M2Mqtt.Messages;
open System
open System.Text

type App() = 
    inherit Application()
    let stack = StackLayout(VerticalOptions = LayoutOptions.Center)
    let label = Label(XAlign = TextAlignment.Center, Text = "F# + mqtt")
    let slider = Slider(Maximum = (float)100, Minimum=(float)1, Value=(float)50)
    let slider2 = Slider(Maximum = (float)100, Minimum=(float)1, Value=(float)50)
    let client = MqttClient("52.29.27.181"); //This is the HiveMQ public broker at broker.mqttdashboard.com
    let clientGuid = Guid.NewGuid()
    let qos : byte array = [|(MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE)|];

    let onSliderValueChanged (valChangedEventArgs : ValueChangedEventArgs) : unit = client.Publish("testtopic" , Encoding.UTF8.GetBytes(string valChangedEventArgs.NewValue) )|> ignore
    let onMessageReceived (data : MqttMsgPublishEventArgs) = 
     Device.BeginInvokeOnMainThread(fun _ -> 
      try
       label.Text <- Encoding.UTF8.GetString(data.Message)
       slider.Value <- float label.Text //My goodness this is awesome
      with
       |  ex -> printfn "%s" ex.Message
     )


    do 
        stack.Children.Add(label)
        stack.Children.Add(slider)
        stack.Children.Add(slider2)
        slider2.ValueChanged.Add(onSliderValueChanged)

        client.Connect(clientGuid.ToString()) |> ignore
        client.Subscribe([| "testtopic" |], qos) |> ignore
        client.MqttMsgPublishReceived.Add(onMessageReceived)

        base.MainPage <- ContentPage(Content = stack)



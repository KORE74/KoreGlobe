using Godot;
using System;

public partial class FssNetworkWindow : Window
{
    Label    UdpIpAddrLabel;
    Label    UdpIpPortLabel;
    LineEdit UdpIpAddrEdit;
    LineEdit UdpIpPortEdit;
    Button   UdpIpConnectButton;

    Label    TcpIpServerAddrLabel;
    Label    TcpIpServerPortLabel;
    LineEdit TcpIpServerAddrEdit;
    LineEdit TcpIpSerfverPortEdit;
    Button   TcpIpServerConnectButton;

    Label    TcpIpClientAddrLabel;
    Label    TcpIpClientPortLabel;
    LineEdit TcpIpClientAddrEdit;
    LineEdit TcpIpClientPortEdit;
    Button   TcpIpClientConnectButton;

    TextEdit NetworkStatusTextEdit;

    Button OkButton;
    Button CancelButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        UdpIpAddrLabel = (Label)FindChild("UdpIpAddrLabel");
        UdpIpPortLabel = (Label)FindChild("UdpIpPortLabel");
        UdpIpAddrEdit  = (LineEdit)FindChild("UdpIpAddrEdit");
        UdpIpPortEdit  = (LineEdit)FindChild("UdpIpPortEdit");
        UdpIpConnectButton = (Button)FindChild("UdpIpConnectButton");

        TcpIpServerAddrLabel = (Label)FindChild("TcpIpServerAddrLabel");
        TcpIpServerPortLabel = (Label)FindChild("TcpIpServerPortLabel");
        TcpIpServerAddrEdit  = (LineEdit)FindChild("TcpIpServerAddrEdit");
        TcpIpSerfverPortEdit = (LineEdit)FindChild("TcpIpSerfverPortEdit");
        TcpIpServerConnectButton = (Button)FindChild("TcpIpServerConnectButton");

        TcpIpClientAddrLabel = (Label)FindChild("TcpIpClientAddrLabel");
        TcpIpClientPortLabel = (Label)FindChild("TcpIpClientPortLabel");
        TcpIpClientAddrEdit  = (LineEdit)FindChild("TcpIpClientAddrEdit");
        TcpIpClientPortEdit  = (LineEdit)FindChild("TcpIpClientPortEdit");
        TcpIpClientConnectButton = (Button)FindChild("TcpIpClientConnectButton");

        NetworkStatusTextEdit = (TextEdit)FindChild("NetworkStatusTextEdit");

        OkButton     = (Button)FindChild("OkButton");
        CancelButton = (Button)FindChild("CancelButton");

        OkButton.Connect("pressed", new Callable(this, "OnOkButtonPressed"));
        CancelButton.Connect("pressed", new Callable(this, "OnCancelButtonPressed"));

        // Connect the close_requested signal to the OnCloseRequested function
        Connect("close_requested", new Callable(this, "OnCancelButtonPressed"));

        var config = FssCentralConfig.Instance;
        string configUdpIpAddr = config.GetParameter<string>("UdpIpAddr", "127.0.0.1");
        int    configUdpIpPort = config.GetParameter<int>("UdpIpPort", 10001);

        LocaliseUIText();
        WriteControlValues();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Data Read and Write
    // --------------------------------------------------------------------------------------------

    private void WriteControlValues()
    {
        var config = FssCentralConfig.Instance;
        UdpIpAddrEdit.Text = config.GetParameter<string>("UdpIpAddr", "127.0.0.1");
        UdpIpPortEdit.Text = config.GetParameter<int>("UdpIpPort", 10001).ToString();

        TcpIpServerAddrEdit.Text = config.GetParameter<string>("TcpIpServerAddr", "127.0.0.1");
        TcpIpSerfverPortEdit.Text = config.GetParameter<int>("TcpIpServerPort", 10002).ToString();

        TcpIpClientAddrEdit.Text = config.GetParameter<string>("TcpIpClientAddr", "127.0.0.1");
        TcpIpClientPortEdit.Text = config.GetParameter<int>("TcpIpClientPort", 10003).ToString();

        NetworkStatusTextEdit.Text = "qq";
    }

    private void SaveControlValues()
    {
        // int udpIpPort = 0;
        // int tcpIpServerPort = 0;
        // int tcpIpClientPort = 0;

        int.TryParse(UdpIpPortEdit.Text, out int udpIpPort);
        int.TryParse(TcpIpSerfverPortEdit.Text, out int tcpIpServerPort);
        int.TryParse(TcpIpClientPortEdit.Text, out int tcpIpClientPort);


        var config = FssCentralConfig.Instance;
        config.SetParameter("UdpIpAddr", UdpIpAddrEdit.Text);
        config.SetParameter("UdpIpPort", udpIpPort);

        config.SetParameter("TcpIpServerAddr", TcpIpServerAddrEdit.Text);
        config.SetParameter("TcpIpServerPort", tcpIpServerPort);

        config.SetParameter("TcpIpClientAddr", TcpIpClientAddrEdit.Text);
        config.SetParameter("TcpIpClientPort", tcpIpClientPort);
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Localisation
    // --------------------------------------------------------------------------------------------

    private void LocaliseUIText()
    {
        OkButton.Text = "OK-2";
        CancelButton.Text = "Cancel-2";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions
    // --------------------------------------------------------------------------------------------

    public void OnOkButtonPressed()
    {
        FssCentralLog.AddEntry("FssNetworkWindow.OnOkButtonPressed");
        Visible = false;

        // Save the control values to the config file
        SaveControlValues();
        FssCentralConfig.Instance.WriteToFile();
    }
    public void OnCancelButtonPressed()
    {
        FssCentralLog.AddEntry("FssNetworkWindow.OnCancelButtonPressed");
        Visible = false;

        // Update the controls from values in the config file
        WriteControlValues();
    }
}

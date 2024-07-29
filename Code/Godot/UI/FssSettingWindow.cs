using Godot;
using System;

public partial class FssSettingWindow : Window
{
    // Controls - Top to bottom
    Label    MapPathLabel;
    LineEdit MapPathLineEdit;
    Label    CapturePathLabel;
    LineEdit CapturePathLineEdit;
    Button   OkButton;
    Button   CancelButton;



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        MapPathLabel        = (Label)FindChild("MapPathLabel");
        MapPathLineEdit     = (LineEdit)FindChild("MapPathLineEdit");
        CapturePathLabel    = (Label)FindChild("CapturePathLabel");
        CapturePathLineEdit = (LineEdit)FindChild("CapturePathLineEdit");
        OkButton            = (Button)FindChild("OkButton");
        CancelButton        = (Button)FindChild("CancelButton");

        // If any of the controls are null, we have a code-vs-UI mismatch, so report this.
        if (MapPathLabel == null || MapPathLineEdit == null || CapturePathLabel == null || CapturePathLineEdit == null || OkButton == null || CancelButton == null)
        {
            FssCentralLog.AddEntry("FssSettingWindow: One or more controls not found");
            return;
        }

        OkButton.Connect("pressed", new Callable(this, "OnOkButtonPressed"));
        CancelButton.Connect("pressed", new Callable(this, "OnCancelButtonPressed"));

        // Connect the close_requested signal to the OnCloseRequested function
        Connect("close_requested", new Callable(this, "OnCancelButtonPressed"));

        UpdateUIText();
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
        // var config = FssCentralConfig.Instance;
        // UdpIpAddrEdit.Text = config.GetParameter<string>("UdpIpAddr", "127.0.0.1");
        // UdpIpPortEdit.Text = config.GetParameter<int>("UdpIpPort", 10001).ToString();

        // TcpIpServerAddrEdit.Text = config.GetParameter<string>("TcpIpServerAddr", "127.0.0.1");
        // TcpIpSerfverPortEdit.Text = config.GetParameter<int>("TcpIpServerPort", 10002).ToString();

        // TcpIpClientAddrEdit.Text = config.GetParameter<string>("TcpIpClientAddr", "127.0.0.1");
        // TcpIpClientPortEdit.Text = config.GetParameter<int>("TcpIpClientPort", 10003).ToString();
    }

    private void SaveControlValues()
    {
        // int udpIpPort = 0;
        // int tcpIpServerPort = 0;
        // int tcpIpClientPort = 0;

        // int.TryParse(UdpIpPortEdit.Text, out int udpIpPort);
        // int.TryParse(TcpIpSerfverPortEdit.Text, out int tcpIpServerPort);
        // int.TryParse(TcpIpClientPortEdit.Text, out int tcpIpClientPort);


        // var config = FssCentralConfig.Instance;
        // config.SetParameter("UdpIpAddr", UdpIpAddrEdit.Text);
        // config.SetParameter("UdpIpPort", udpIpPort);

        // config.SetParameter("TcpIpServerAddr", TcpIpServerAddrEdit.Text);
        // config.SetParameter("TcpIpServerPort", tcpIpServerPort);

        // config.SetParameter("TcpIpClientAddr", TcpIpClientAddrEdit.Text);
        // config.SetParameter("TcpIpClientPort", tcpIpClientPort);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Localisation
    // --------------------------------------------------------------------------------------------

    private void UpdateUIText()
    {
        OkButton.Text = "OK-2";
        CancelButton.Text = "Cancel-2";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions
    // --------------------------------------------------------------------------------------------

    public void OnOkButtonPressed()
    {
        FssCentralLog.AddEntry("FssSettingWindow.OnOkButtonPressed");

        Visible = false;
    }
    public void OnCancelButtonPressed()
    {
        FssCentralLog.AddEntry("FssSettingWindow.OnCancelButtonPressed");

        Visible = false;
    }
}

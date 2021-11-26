using Godot;
using System;

public class MainMenu : Control
{
    public MainMenu()
    {
    }

    private const int DefaultPort = 8910; // An arbitrary number.
    private const int MaxNumberOfPeers = 1; // How many people we want to have in a game

    private LineEdit address;
    private Button hostButton;
    private Button joinButton;
    private Label statusOk;
    private Label statusFail;
    private NetworkedMultiplayerENet peer;

    public override void _Ready()
    {
        // Get nodes - the generic is a class, argument is node path.
        address = GetNode<LineEdit>("LobbyPanel/Address");
        hostButton = GetNode<Button>("LobbyPanel/HostButton");
        joinButton = GetNode<Button>("LobbyPanel/JoinButton");
        statusOk = GetNode<Label>("LobbyPanel/StatusOk");
        statusFail = GetNode<Label>("LobbyPanel/StatusFail");

        // Connect all callbacks related to networking.
        // Note: Use snake_case when talking to engine API.
        GetTree().Connect("network_peer_connected", this, nameof(PlayerConnected));
        GetTree().Connect("network_peer_disconnected", this, nameof(PlayerDisconnected));
        GetTree().Connect("connected_to_server", this, nameof(ConnectedOk));
        GetTree().Connect("connection_failed", this, nameof(ConnectedFail));
        GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
    }

    // Network callbacks from SceneTree

    // Callback from SceneTree.
    private void PlayerConnected(int id)
    {
        // Someone connected, start the game!
        var stage = ResourceLoader.Load<PackedScene>("res://Scenes/DemoScene.tscn").Instance();

        // Connect deferred so we can safely erase it from the callback.
        // stage.Connect("GameFinished", this, nameof(EndGame), new Godot.Collections.Array(),
        //     (int) ConnectFlags.Deferred);
        
        GetTree().Root.AddChild(stage);
        Globals.Player2Id = id;
        Hide();
    }

    private void PlayerDisconnected(int id)
    {
        EndGame(GetTree().IsNetworkServer() ? "Client disconnected" : "Server disconnected");
    }

    // Callback from SceneTree, only for clients (not server).
    private void ConnectedOk()
    {
        // This function is not needed for this project.
    }

    // Callback from SceneTree, only for clients (not server).
    private void ConnectedFail()
    {
        SetStatus("Couldn't connect", false);

        GetTree().NetworkPeer = null; // Remove peer.
        hostButton.Disabled = false;
        joinButton.Disabled = false;
    }

    private void ServerDisconnected()
    {
        EndGame("Server disconnected");
    }

    // Game creation functions

    private void EndGame(string withError = "")
    {
        if (HasNode("/root/Pong"))
        {
            // Erase immediately, otherwise network might show
            // errors (this is why we connected deferred above).
            GetNode("/root/Pong").Free();
            Show();
        }

        GetTree().NetworkPeer = null; // Remove peer.
        hostButton.Disabled = false;
        joinButton.Disabled = false;

        SetStatus(withError, false);
    }

    private void SetStatus(string text, bool isOk)
    {
        // Simple way to show status.
        if (isOk)
        {
            statusOk.Text = text;
            statusFail.Text = "";
        }
        else
        {
            statusOk.Text = "";
            statusFail.Text = text;
        }
    }

    private void OnHostPressed()
    {
        peer = new NetworkedMultiplayerENet();
        peer.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.RangeCoder;
        var err = peer.CreateServer(DefaultPort, MaxNumberOfPeers);
        if (err != Error.Ok)
        {
            // Is another server running?
            SetStatus("Can't host, address in use.", false);
            return;
        }

        GetTree().NetworkPeer = peer;
        hostButton.Disabled = true;
        joinButton.Disabled = true;
        SetStatus("Waiting for player...", true);
    }

    private void OnJoinPressed()
    {
        string ip = address.Text;
        if (!ip.IsValidIPAddress())
        {
            SetStatus("IP address is invalid", false);
            return;
        }

        peer = new NetworkedMultiplayerENet();
        peer.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.RangeCoder;
        peer.CreateClient(ip, DefaultPort);
        GetTree().NetworkPeer = peer;
        SetStatus("Connecting...", true);
    }
}
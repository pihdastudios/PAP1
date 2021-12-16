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
  public Button hostButton;
  public Button joinButton;
  private Label statusOk;
  private Label statusFail;
  private TextureRect background;
  private Panel lobbyPanel;
  public NetworkedMultiplayerENet peer;

  public override void _Ready()
  {
	// Get nodes - the generic is a class, argument is node path.
	address = GetNode<LineEdit>("LobbyPanel/VBoxContainer/HBoxContainer2/Address");
	hostButton = GetNode<Button>("LobbyPanel/VBoxContainer/HBoxContainer/HostButton");
	joinButton = GetNode<Button>("LobbyPanel/VBoxContainer/HBoxContainer/JoinButton");
	statusOk = GetNode<Label>("LobbyPanel/VBoxContainer/CenterContainer/StatusOk");
	statusFail = GetNode<Label>("LobbyPanel/VBoxContainer/CenterContainer/StatusFail");
	background = GetNode<TextureRect>("TextureRect");
	lobbyPanel = GetNode<Panel>("LobbyPanel");
	ResizeMenu();

	// Connect all callbacks related to networking.
	// Note: Use snake_case when talking to engine API.
	GetTree().Connect("network_peer_connected", this, nameof(PlayerConnected));
	GetTree().Connect("network_peer_disconnected", this, nameof(PlayerDisconnected));
	GetTree().Connect("connection_failed", this, nameof(ConnectedFail));
	GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
	GetTree().Root.Connect("size_changed", this, nameof(ResizeMenu));
  }

  private void ResizeMenu()
  {
	background.RectSize = GetViewportRect().Size;

	// Panel size
	if (lobbyPanel.RectSize.x < 300)
	{
	  lobbyPanel.AnchorLeft = 0.2f;
	  lobbyPanel.AnchorRight = 0.8f;
	}
	else
	{
	  lobbyPanel.AnchorLeft = 0.28f;
	  lobbyPanel.AnchorRight = 0.72f;
	}
	var newSizeY = background.RectSize.y * 0.5f;
	if (newSizeY > 207)
	  lobbyPanel.RectSize = new Vector2(lobbyPanel.RectSize.x, 207);

	if (newSizeY < 190)
	  lobbyPanel.RectSize = new Vector2(lobbyPanel.RectSize.x, 190);
  }

  private void PlayerConnected(int id)
  {
	Globals.PeerId = id;
	// Someone connected, start the game!
	var stage = GD.Load<PackedScene>("res://Scenes/DemoScene.tscn").Instance();
	GetTree().Root.AddChild(stage);
	Hide();
  }

  private void PlayerDisconnected(int id)
  {
	var demoScene = GetTree().Root.GetNode<DemoScene>("DemoScene");
	if (!demoScene.isGameOver)
	  EndGame("Your opponent has left the game");
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
	EndGame("Your opponent has left the game");
  }

  public void SetStatus(string text, bool isOk)
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
	Globals.CurrentRole = Globals.Role.Attacker;
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
	Globals.CurrentRole = Globals.Role.Defender;
	var ip = address.Text;
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

  public void EndGame(String message = "", bool isOk = false)
  {
	GetTree().NetworkPeer = null;
	peer.CloseConnection();
	SetStatus(message, isOk);
	hostButton.Disabled = false;
	joinButton.Disabled = false;
	Show();

	var demoScene = GetTree().Root.GetNode("DemoScene");
	demoScene?.QueueFree();
	Input.SetMouseMode(Input.MouseMode.Visible);
  }
}

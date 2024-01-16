extends Node2D

signal piece_selected(piece)
@onready var icon_path = $Icon
var slot_ID := -1
var type : int



# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func load_icon(piece_name) -> void:
	icon_path.texture = load(DataHandler.assets[piece_name])



func _on_icon_gui_input(event: InputEvent) -> void:
	if event.is_action_pressed("mouse_left"):
		emit_signal("piece_selected", self)

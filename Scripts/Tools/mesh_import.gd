tool # Needed so it runs in editor
extends EditorScenePostImport

# This sample changes all node names

# Called right after the scene is imported and gets the root node
var depth: int = 0
func post_import(scene : Object) -> Object:
	# Change all node names to "modified_[oldnodename]"
	iterate(scene)
	return scene # Remember to return the imported scene

func iterate(node : Object):
	depth = depth + 1
	if node != null:
#		culling
		if node is MeshInstance and depth == 2 and (String(node.name).find("Door") == -1 and String(node.name).find("Window") == -1):
			node.add_to_group("Occlusion_Culling", true)
		elif node is MeshInstance and depth == 3 and (String(node.name).find("Door") == -1 or String(node.name).find("Window") == -1):
#			if node.get_parent().
			if not "Occlusion_Culling" in node.get_parent().get_groups():
				node.add_to_group("Occlusion_Culling", true)
			

		if node is MeshInstance and not (String(node.name).find("Handle") != -1 or String(node.name).find("Floor") != -1 or (String(node.name).find("Door") != -1 and not String(node.name).find("Door ") != -1) or String(node.name).find("-colonly") != -1):
			if String(node.name).find("Wall") != -1 or String(node.name).find("Ceiling") != -1 or String(node.name).find("Kitchen") != -1 or String(node.name).find("Slab") != -1 or String(node.name).find("Fence") != -1 or String(node.name).find("Stair") != -1:
				node.create_trimesh_collision()
			else:
				node.create_convex_collision()
			#node.set_gi_mode(1)
			node.use_in_baked_light = true
		elif node is MeshInstance and String(node.name).find("Floor") != -1:
			node.use_in_baked_light = true
			print(node.get_flag(MeshInstance.FLAG_USE_BAKED_LIGHT))

		for child in node.get_children():
			iterate(child)
			depth = depth - 1

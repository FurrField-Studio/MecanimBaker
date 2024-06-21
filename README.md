# Mecanim Baker

What is Mecanim Baker?

Mecanim baker is a tool to bake animation controller data into single SO.
It bakes state names, parameter names and all values that appear in animation transitions.

<img align="center" src="./Images~/so.jpg">

To bake data from animator controller:
1. Create AnimatorParamsReader found in ``Create/MecanimBaker/AnimatorParamsReader``
2. Assign AnimatorController in SO
3. Click "Bake"

This will grab all states, params and state transitions and bake it into POCOs that you will be able to choose through ``AnimationParamEntry`` or ``AnimationParamSetterEntry``.

<img align="center" src="./Images~/entries.jpg">

The ``AnimationParamEntry`` allows you to set which Parameter you want to read/write from code.
The ``AnimationParamSetterEntry`` has almost the same functionality but it sets the value that you choose from inspector and also allows to set parameter to default value.

# Installation

- open <kbd>Window/Package Manager</kbd>
- click <kbd>+</kbd>
- click <kbd>Add package from git URL</kbd> or <kbd>Add package by name</kbd>
- Add `https://github.com/FurrField-Studio/MecanimBaker.git` in Package Manager

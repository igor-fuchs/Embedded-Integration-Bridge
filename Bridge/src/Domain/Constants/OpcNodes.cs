namespace Bridge.Domain.Constants;

/// <summary>
/// Contains OPC UA Node IDs for monitoring industrial equipment.
/// </summary>
public static class OpcNodes
{
    /// <summary>
    /// NodeIds loaded from the signals file used for monitoring.
    /// </summary>
    public static IReadOnlyList<string> NodeIdsToMonitor { get; } =
    [
        // ST005 - Buffer Station
        @"ns=3;s=""ST005_BUFFER_SHM"".""CONVEYOR_SHM"".""STATUS"".""RUNNING""",
        @"ns=3;s=""ST005_BUFFER_FB_IDB"".""EMITTER_FB"".""NextIsGreen""",
        @"ns=3;s=""ST005_BUFFER_FB_IDB"".""EMITTER_FB"".""NextIsBlue""",
        @"ns=3;s=""ST005_BUFFER_FB_IDB"".""EMITTER_FB"".""NextIsMetal""",
        @"ns=3;s=""ST005_BUFFER_SHM"".""EMITTER_SHM"".""SENSOR_STATUS",
        @"ns=3;s=""ST005_BUFFER_FB_IDB"".""EMITTER_FB"".""EnablePart"".""Green""",
        @"ns=3;s=""ST005_BUFFER_FB_IDB"".""EMITTER_FB"".""EnablePart"".""Blue""",
        @"ns=3;s=""ST005_BUFFER_FB_IDB"".""EMITTER_FB"".""EnablePart"".""Metal""",
        @"ns=3;s=""ST005_BUFFER_SHM"".""ROBOT_SHM"".""STATUS"".""HOME_POSITION""",
        @"ns=3;s=""ST005_BUFFER_SHM"".""ROBOT_SHM"".""STATUS"".""DROP_POSITION""",
        @"ns=3;s=""ST005_BUFFER_SHM"".""ROBOT_SHM"".""STATUS"".""PICK_POSITION""",
        @"ns=3;s=""ST005_BUFFER_SHM"".""ROBOT_SHM"".""STATUS"".""PART_DEPOSITED""",
        @"ns=3;s=""ST005_BUFFER_FB_IDB"".""ROBOT_FB"".""GoToPick""",
        @"ns=3;s=""ST005_BUFFER_FB_IDB"".""ROBOT_FB"".""GoToDrop""",
        @"ns=3;s=""ST005_BUFFER_FB_IDB"".""ROBOT_FB"".""GoToHome""",
        @"ns=3;s=""ST005_BUFFER_FB_IDB"".""ROBOT_FB"".""GoToAntDrop""",
        @"ns=3;s=""ST005_BUFFER_FB_IDB"".""ROBOT_FB"".""GoToPreDrop""",
        @"ns=3;s=""ST005_BUFFER_FB_IDB"".""ROBOT_FB"".""TurnOnGrab""",
        @"ns=3;s=""ST005_BUFFER_SHM"".""SENSOR_SHM"".""ON""",

        // ST007 - Buffer Station
        @"ns=3;s=""ST007_BUFFER_SHM"".""CONVEYOR_SHM"".""STATUS"".""RUNNING""",
        @"ns=3;s=""ST007_BUFFER_SHM"".""CONVEYOR_SHM"".""STATUS"".""FORCED_STOP""",
        @"ns=3;s=""ST007_BUFFER_FB_IDB"".""EMITTER_FB"".""NextIsGreen""",
        @"ns=3;s=""ST007_BUFFER_FB_IDB"".""EMITTER_FB"".""NextIsBlue""",
        @"ns=3;s=""ST007_BUFFER_FB_IDB"".""EMITTER_FB"".""NextIsMetal""",
        @"ns=3;s=""ST007_BUFFER_FB_IDB"".""EMITTER_FB"".""EnablePart"".""Green""",
        @"ns=3;s=""ST007_BUFFER_FB_IDB"".""EMITTER_FB"".""EnablePart"".""Blue""",
        @"ns=3;s=""ST007_BUFFER_FB_IDB"".""EMITTER_FB"".""EnablePart"".""Metal""",
        @"ns=3;s=""ST007_BUFFER_SHM"".""SENSOR_SHM"".""ON""",
        @"ns=3;s=""ST007_BUFFER_SHM"".""ROBOT_SHM"".""STATUS"".""HOME_POSITION""",
        @"ns=3;s=""ST007_BUFFER_SHM"".""ROBOT_SHM"".""STATUS"".""DROP_POSITION""",
        @"ns=3;s=""ST007_BUFFER_SHM"".""ROBOT_SHM"".""STATUS"".""PICK_POSITION""",
        @"ns=3;s=""ST007_BUFFER_SHM"".""ROBOT_SHM"".""STATUS"".""PART_DEPOSITED""",
        @"ns=3;s=""ST007_BUFFER_FB_IDB"".""ROBOT_FB"".""GoToPick""",
        @"ns=3;s=""ST007_BUFFER_FB_IDB"".""ROBOT_FB"".""GoToDrop""",
        @"ns=3;s=""ST007_BUFFER_FB_IDB"".""ROBOT_FB"".""GoToHome""",
        @"ns=3;s=""ST007_BUFFER_FB_IDB"".""ROBOT_FB"".""GoToAntDrop""",
        @"ns=3;s=""ST007_BUFFER_FB_IDB"".""ROBOT_FB"".""GoToPreDrop""",
        @"ns=3;s=""ST007_BUFFER_FB_IDB"".""ROBOT_FB"".""TurnOnGrab""",

        // ST010 - Separator Station
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""ENTRY_SENSOR_SHM"".""ON""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""ENTRY_CONVEYOR_SHM"".""STATUS"".""RUNNING""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""ACTUATOR_POS_A_SHM"".""STATUS"".""ADVANCE_POSITION""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""ACTUATOR_POS_A_SHM"".""STATUS"".""RETRACT_POSITION""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""ACTUATOR_POS_B_SHM"".""STATUS"".""ADVANCE_POSITION""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""ACTUATOR_POS_B_SHM"".""STATUS"".""RETRACT_POSITION""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""EXIT_CONVEYOR_SHM"".""STATUS"".""RUNNING""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""EXIT_SENSOR_A_SHM"".""ON""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""ACTUATOR_A_SHM"".""STATUS"".""ADVANCE_POSITION""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""ACTUATOR_A_SHM"".""STATUS"".""RETRACT_POSITION""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""EXIT_SENSOR_B_SHM"".""ON""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""ACTUATOR_B_SHM"".""STATUS"".""ADVANCE_POSITION""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""ACTUATOR_B_SHM"".""STATUS"".""RETRACT_POSITION""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""EXIT_SENSOR_C_SHM"".""ON""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""ACTUATOR_C_SHM"".""STATUS"".""ADVANCE_POSITION""",
        @"ns=3;s=""ST010_SEPARATOR_SHM"".""ACTUATOR_C_SHM"".""STATUS"".""RETRACT_POSITION""",
    ];
}

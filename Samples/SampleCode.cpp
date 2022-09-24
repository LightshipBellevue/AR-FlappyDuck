
std::string FFramePose::ToJson(int32 FrameID) const
{
	std::string ret = "";
	std::string endl = "\n";

	ret += tabs(3) + "{" + endl;
	ret += tabs(4) + "\"Frame\" : " + TCHAR_TO_ANSI(*FString::Printf(TEXT("%i"), FrameID)) + "," + endl;

	ret += tabs(4) + "\"FrameTime\" : " + TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), FrameTime)) + "," + endl;

	ret += tabs(4) + "\"Speed\": " + TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), Speed));

	if (Bones.Num() > 0)
	{
		ret += "," + endl;
		ret += tabs(4) + "\"Bones\" : " + endl;
		ret += tabs(4) + "[" + endl;

		int32 BoneCt = 0;
		for (const auto BoneList : Bones)
		{

			FVector pos = BoneList.Value.GetLocation();
			FQuat rot = BoneList.Value.GetRotation();
			FVector scale = BoneList.Value.GetScale3D();

			ret += tabs(5) + "{\"Name\" : \"" + TCHAR_TO_ANSI(*BoneList.Key) + "\","
				+ "\"XForm\":{"
				+ "\"Pos\":["
				+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), pos.X)) + ","
				+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), pos.Y)) + ","
				+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), pos.Z)) + "],"
				+ "\"Rot\": ["
				+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), rot.X)) + ","
				+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), rot.Y)) + ","
				+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), rot.Z)) + ","
				+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), rot.W)) + "]"
				+ "}}";

			if (++BoneCt < Bones.Num())
				ret += ",";

			ret += "\n";
		}
		ret += tabs(4) + "]";		//end bones array
	}

	//Write the list of input events which happened this frame
	if (InputEvents.Num() > 0)
	{
		ret += "," + endl;
		ret += tabs(4) + "\"InputEvents\": [";
		int32 ct = 0;
		for (const FString InputEvent : InputEvents)
		{
			ret += "\"";
			ret += TCHAR_TO_ANSI(*InputEvent);
			ret += "\"";
			if (++ct < InputEvents.Num()) ret += ",";
		}
		ret += "]";
	}

	//write the list of analog axis events which happened this frame
	if (AxisEvents.Num() > 0)
	{
		ret += "," + endl;
		ret += tabs(4) + "\"AxisEvents\": [";
		int32 ct = 0;
		for (const auto AxisEvent : AxisEvents)
		{
			ret += "{\"Key\":\"";
			ret += TCHAR_TO_ANSI(*AxisEvent.Key);
			ret += "\",\"Val\":";
			ret += TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), AxisEvent.Value));
			ret += "}";
			if (++ct < AxisEvents.Num()) ret += ",";
		}
		ret += "]";
	}

	ret += endl + tabs(3) + "}";		//end FramePoses array


	return ret;
}


void FPoseSkel::FromJson(picojson::value data)
{
	if (data.contains("PoseSkeleton"))
	{
		picojson::value PoseSkeleton = data.get("PoseSkeleton");

		if (PoseSkeleton.contains("GameTime"))
			StartGametime = (float)PoseSkeleton.get("GameTime").get<double>();

		if (PoseSkeleton.contains("RootPos"))
		{
			picojson::array& RootArr = PoseSkeleton.get("RootPos").get<picojson::array>();
			float x = RootArr[0].get<double>();
			float y = RootArr[1].get<double>();
			float z = RootArr[2].get<double>();
			RootPos.X = x;
			RootPos.Y = y;
			RootPos.Z = z;
		}

		if (PoseSkeleton.contains("RootRot"))
		{
			picojson::array& RootArr = PoseSkeleton.get("RootRot").get<picojson::array>();
			float x = RootArr[0].get<double>();
			float y = RootArr[1].get<double>();
			float z = RootArr[2].get<double>();
			float w = RootArr[3].get<double>();

			FQuat tmp = FQuat(x, y, z, w);
			RootRot = tmp.Rotator();
		}

		if (PoseSkeleton.contains("Poses"))
		{
			picojson::array& PoseArr = PoseSkeleton.get("Poses").get<picojson::array>();
			for (picojson::array::const_iterator it = PoseArr.begin(); it != PoseArr.end(); it++)
			{
				picojson::value Pose = *it;
				if (Pose.contains("Frame"))
				{
					int32 id = (int32)Pose.get("Frame").get<double>();
					if (LastFrame < id) LastFrame = id;

					TPair<int32, FFramePose> Frame;
					Frame.Key = id;
					if (Pose.contains("Speed"))
						Frame.Value.Speed = (float)Pose.get("Speed").get<double>();

					if (Pose.contains("FrameTime"))
						Frame.Value.FrameTime = (float)Pose.get("FrameTime").get<double>();

					if (Pose.contains("Bones"))
					{
						Frame.Value.FromJson(Pose.get("Bones").get<picojson::array>());
					}

					if (Pose.contains("AxisEvents"))
					{
						picojson::array& AxisEventsArr = Pose.get("AxisEvents").get<picojson::array>();
						for (picojson::array::const_iterator it2 = AxisEventsArr.begin(); it2 != AxisEventsArr.end(); it2++)
						{
							picojson::value AxisEvent = *it2;
							int32 success = 0;
							if (AxisEvent.contains("Key"))
							{
								success++;
							}
							if (AxisEvent.contains("Val"))
							{
								success++;
							}

							if (success == 2)
							{
								std::string key = AxisEvent.get("Key").to_str();
								FString fkey = FString(key.c_str());
								float val = (float)AxisEvent.get("Val").get<double>();
								Frame.Value.AxisEvents.Add(TTuple<FString, float>(fkey, val));
							}
						}
					}

					if (Pose.contains("InputEvents"))
					{
						picojson::array& InputEventsArr = Pose.get("InputEvents").get<picojson::array>();
						for (picojson::array::const_iterator it2 = InputEventsArr.begin(); it2 != InputEventsArr.end(); it2++)
						{
							picojson::value InputEvent = *it2;
							std::string val = InputEvent.to_str();
							FString fval = FString(val.c_str());
							Frame.Value.InputEvents.Add(fval);
						}
					}
					FrameList.Add(Frame);
				}
			}
		}
	}
}

std::string FPoseSkel::ToJson() const
{
	std::string ret = "";
	std::string endl = "\n";
	ret += tabs(1) + "\"PoseSkeleton\" : " + endl;

	ret += tabs(1) + "{" + endl;
	ret += tabs(2) + "\"GameTime\" : " + TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), StartGametime)) + "," + endl;
	ret += tabs(2) + "\"FrameCount\" : " + TCHAR_TO_ANSI(*FString::Printf(TEXT("%i"), GetFrameCount())) + "," + endl;
	ret += tabs(2) + "\"RootPos\" : ["
		+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), RootPos.X)) + ","
		+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), RootPos.Y)) + ","
		+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), RootPos.Z)) + "]," + endl;

	ret += tabs(2) + "\"RootRot\" : ["
		+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), RootRot.Quaternion().X)) + ","
		+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), RootRot.Quaternion().Y)) + ","
		+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), RootRot.Quaternion().Z)) + ","
		+ TCHAR_TO_ANSI(*FString::Printf(TEXT("%f"), RootRot.Quaternion().W)) + "]," + endl;


	ret += tabs(2) + "\"Poses\" : " + endl;
	ret += tabs(2) + "[" + endl;

	//note that the FramePoses is a sparse array so we can't iterate linearly.
	int32 PoseCt = 0;
	for (const auto& Poses : FrameList)
	{


		ret += Poses.Value.ToJson(Poses.Key);


		if (++PoseCt < FrameList.Num())
			ret += ",";
		ret += endl;

	}
	ret += tabs(2) + "]" + endl;
	ret += tabs(1) + "}" + endl;

	return ret;
}

std::string tabs(int32 count)
{
	std::string ret = "";
	for (int a = 0; a < count; a++)
	{
		ret += "\t";
	}

	return ret;
}
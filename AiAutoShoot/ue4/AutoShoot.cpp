// Copyright Epic Games, Inc. All Rights Reserved.

#include "AutoShoot.h"
#include "AutoShootStyle.h"
#include "AutoShootCommands.h"
#include "Misc/MessageDialog.h"
#include "ToolMenus.h"

////
#include "Engine.h"
#include "Runtime/Engine/Classes/Kismet/GameplayStatics.h"
#include "LevelEditor.h"
#include "Kismet/KismetMathLibrary.h"
#include "ShiftLensCineCameraActor.h"
#include "ShiftLensCineCameraComponent.h"


static const FName AutoShootTabName("AutoShoot");

#define LOCTEXT_NAMESPACE "FAutoShootModule"

void FAutoShootModule::StartupModule()
{
	// This code will execute after your module is loaded into memory; the exact timing is specified in the .uplugin file per-module
	
	FAutoShootStyle::Initialize();
	FAutoShootStyle::ReloadTextures();

	FAutoShootCommands::Register();
	
	PluginCommands = MakeShareable(new FUICommandList);

	PluginCommands->MapAction(
		FAutoShootCommands::Get().PluginAction,
		FExecuteAction::CreateRaw(this, &FAutoShootModule::PluginButtonClicked),
		FCanExecuteAction());

	UToolMenus::RegisterStartupCallback(FSimpleMulticastDelegate::FDelegate::CreateRaw(this, &FAutoShootModule::RegisterMenus));
}

void FAutoShootModule::ShutdownModule()
{
	// This function may be called during shutdown to clean up your module.  For modules that support dynamic reloading,
	// we call this function before unloading the module.

	UToolMenus::UnRegisterStartupCallback(this);

	UToolMenus::UnregisterOwner(this);

	FAutoShootStyle::Shutdown();

	FAutoShootCommands::Unregister();
}

// ボタンが押されたとき
void FAutoShootModule::PluginButtonClicked()
{
	//
	UWorld* World = nullptr;
	for (const FWorldContext& Context : GEngine->GetWorldContexts())
	{
		UWorld* ThisWorld = Context.World();
		if (!ThisWorld)
		{
			continue;
		}
		else if (Context.WorldType == EWorldType::PIE)
		{
			World = ThisWorld;
			break;
		}
		else if (Context.WorldType == EWorldType::Editor)
		{
			World = ThisWorld;
		}
	}

	//
	//TSubclassOf<ACameraActor> findClass;
	//findClass = ACameraActor::StaticClass();
	TSubclassOf<AShiftLensCineCameraActor> findClass;
	findClass = AShiftLensCineCameraActor::StaticClass();
	TArray<AActor*> Cameras;
	UGameplayStatics::GetAllActorsOfClass(World, findClass, Cameras);

	if (Cameras.Num())
	{
		//ACameraActor* camera = Cast<ACameraActor>(Cameras[0]);
		AShiftLensCineCameraActor* camera = Cast<AShiftLensCineCameraActor>(Cameras[0]);

		FVector orgLocation = camera->GetActorLocation();

		float height = 100;

		//FVector CenterLocation(1200, 0, 500);
		FVector CenterLocation(0, 0, height);
		//FVector CenterLocation(FVector::ZeroVector);

		int startX = -2000;
		int endX = 2000;
		int startY = -200;
		int endY = 2000;
		int startZ = 100;
		int endZ = 1100;
		int step = 500;
		//for (int z = startZ; z <= endZ; z += step) {
		//for (double s = 0.0; s < 1.0; s += 0.3) {
			for (int y = startY; y <= endY; y += step) {
				for (int x = startX; x <= endX; x += step) {

					// 座標を差し替えます
					FVector Location(x, y, height);
					FRotator Rotation = UKismetMathLibrary::FindLookAtRotation(Location, CenterLocation);
					camera->SetActorLocation(Location);
					camera->SetActorRotation(Rotation);
					//UShiftLensCineCameraComponent* cameracompo = Cast<UShiftLensCineCameraComponent>(camera->GetCameraComponent());
					//cameracompo->SetShiftLens(s);

					// ここでスクリーンショットが取れます
					for (FEditorViewportClient* ViewportClient : GEditor->GetAllViewportClients())
					{
						FSceneViewStateInterface* ViewportParentView = ViewportClient->ViewState.GetReference();
						if (ViewportClient->ViewIndex > 0 && ViewportClient->IsPerspective()) {
							// カメラに座標を適用
							ViewportClient->ViewFOV = 90.0f;
							ViewportClient->SetViewLocation(Location);
							//ViewportClient->SetLookAtLocation(CenterLocation, true);
							ViewportClient->SetViewRotation(Rotation);

							//FString PrintString = FString::Printf(TEXT(" %.2f"), Location);
							GEngine->AddOnScreenDebugMessage(-1, 1.0f, FColor::White, Location.ToString());
							////
							// Send notification about actors that may have changed
							ULevel::LevelDirtiedEvent.Broadcast();

							// Update the details window with the actors we have just selected
							//GUnrealEd->UpdateFloatingPropertyWindowsFromActorList(SelectedActors);

							// Redraw viewports to show new camera
							GEditor->RedrawAllViewports();
							////

							FPlatformProcess::Sleep(0.2f);
							ViewportClient->TakeScreenshot(ViewportClient->Viewport, true);
							FPlatformProcess::Sleep(0.2f);
						}
					}
				}
			}
		//}

		// 元の座標に戻す
		camera->SetActorLocation(orgLocation);
	}
}

void FAutoShootModule::RegisterMenus()
{
	// Owner will be used for cleanup in call to UToolMenus::UnregisterOwner
	FToolMenuOwnerScoped OwnerScoped(this);

	{
		UToolMenu* Menu = UToolMenus::Get()->ExtendMenu("LevelEditor.MainMenu.Window");
		{
			FToolMenuSection& Section = Menu->FindOrAddSection("WindowLayout");
			Section.AddMenuEntryWithCommandList(FAutoShootCommands::Get().PluginAction, PluginCommands);
		}
	}

	{
		UToolMenu* ToolbarMenu = UToolMenus::Get()->ExtendMenu("LevelEditor.LevelEditorToolBar");
		{
			FToolMenuSection& Section = ToolbarMenu->FindOrAddSection("Settings");
			{
				FToolMenuEntry& Entry = Section.AddEntry(FToolMenuEntry::InitToolBarButton(FAutoShootCommands::Get().PluginAction));
				Entry.SetCommandList(PluginCommands);
			}
		}
	}
}

#undef LOCTEXT_NAMESPACE
	
IMPLEMENT_MODULE(FAutoShootModule, AutoShoot)

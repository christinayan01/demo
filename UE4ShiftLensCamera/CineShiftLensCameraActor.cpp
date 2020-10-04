// Fill out your copyright notice in the Description page of Project Settings.


#include "CineShiftLensCameraActor.h"
#include "Engine.h"
#include "YanaiLocalPlayer.h"
#if 0
#include "Slate/SceneViewport.h"
#include "Editor/UnrealEdTypes.h"
#include "EditorWorldExtension.h"
#include "Editor.h"
#include "EditorViewportClient.h"
#endif //0

//
ACineShiftLensCameraActor::ACineShiftLensCameraActor(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	SetActorTickEnabled(true);

	this->ShiftLens = 0.0f;
}

//
void ACineShiftLensCameraActor::SetShiftLens(const float& InShiftLens) 
{ 
	this->ShiftLens = InShiftLens; 
}

//
void ACineShiftLensCameraActor::Tick(float DeltaTime)
{
	UpdateShiftLens();
	Super::Tick(DeltaTime);
}

// シフトレンズカメラの更新
void ACineShiftLensCameraActor::UpdateShiftLens()
{
	APlayerController* PlayerController = nullptr;

	for (FConstPlayerControllerIterator Iterator = GetWorld()->GetPlayerControllerIterator(); Iterator; ++Iterator)
	{
		PlayerController = Iterator->Get();
		if (PlayerController && PlayerController->PlayerCameraManager)
		{
			AActor* ViewTarget = PlayerController->GetViewTarget();
			if (ViewTarget == this) 	// 暫定：ビューポート表示に使用しているカメラだけ処理できる
			{
				ULocalPlayer* LocalPlayer = PlayerController->GetLocalPlayer();
				if (LocalPlayer) 
				{
					UYanaiLocalPlayer* YanaiLocal = Cast<UYanaiLocalPlayer>(LocalPlayer);
					if (YanaiLocal) 
					{
						YanaiLocal->SetShiftLens(this->ShiftLens);
					}
				}
			}
		}
	}
	return;


#if 0
	// test code ->

	UGameViewportClient* GameViewport = GEngine->GameViewport;
	//TSharedPtr<SViewport> ViewportWidget = ViewportClient->GetGameViewportWidget();
	//FIntPoint ScreenshotSize = ViewportClient->GetGameViewport()->GetSizeXY();
	//TArray<FColor> OutColorData;
	/*
	if (ViewportWidget.IsValid())
	{
		//if (ViewportWidget->ShouldRenderDirectly())
		//{
		const FSceneViewport* GameViewport = ViewportClient->GetGameViewport();
		FSceneViewFamilyContext ViewFamily(FSceneViewFamily::ConstructionValues(
			ViewportClient->Viewport,
			ViewportClient->GetScene(),
			ViewportClient->EngineShowFlags)
			.SetRealtimeUpdate(ViewportClient->IsRealtime()));
		GameViewport->CalcSceneView(&ViewFamily);
		//FSceneView* View = ViewportClient->CalcSceneView(&ViewFamily);
		//ReadBackbuffer(GameViewport, &OutColorData);
		//}
	}
	*/

	//const FSceneViewport* GameViewport = ViewportClient->GetGameViewport();
//	const FSceneViewport* WorldViewport = GetWorld()->GetGameViewport();
	{
		FSceneViewFamilyContext ViewFamily(FSceneViewFamily::ConstructionValues(
			GameViewport->Viewport,
			GetWorld()->Scene, //ViewportClient->GetScene(),
			GameViewport->EngineShowFlags)
			.SetRealtimeUpdate(true)//GameViewport->IsRealtime())
		);

		FSceneView* SceneView = 0;
		ULocalPlayer* LocalPlayer = Cast<ULocalPlayer>(PlayerController->Player);// Cast<ULocalPlayer>(GetOwningPlayerController()->Player);
		if (LocalPlayer) {
			FVector ViewLocation;
			FRotator ViewRotation;
			SceneView = LocalPlayer->CalcSceneView(&ViewFamily, /*out*/ ViewLocation, /*out*/ ViewRotation, LocalPlayer->ViewportClient->Viewport);

			// あおり補正値をプロジェクションマトリックスに埋め込む->dame
			if (SceneView) {
				FMatrix CurrentMatrix = SceneView->ViewMatrices.GetProjectionMatrix();
				FMatrix NewMatrix = CurrentMatrix;
				NewMatrix.M[2][1] = this->ShiftLens * -1.0f;
				SceneView->UpdateProjectionMatrix(NewMatrix);
			}
			//FLinearColor c = SceneView->BackgroundColor;
			//float a = SceneView->FOV;
			//a = a +1;
			//a = a - 1;
			GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::White, FString::Printf(TEXT("FOV=%f"), SceneView->FOV));
			GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::White, FString::Printf(TEXT("Shift=%f"), this->ShiftLens));
		}
	}

	////

/*
	if (!GEngine)
		return;
	
	const APawn* OwningPawn = Cast<APawn>(GetOwner());
	const AController* OwningController = OwningPawn ? OwningPawn->GetController() : nullptr;
	if (OwningController)
	{
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::White, FString("OwningController camera!"));
		if (OwningController->IsLocalPlayerController())
		{
			GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::White, FString("ownder camera!"));
		}
	}
	
	UWorld* World = GetWorld();
	if (!World)
		return;
*/

/*
	// 起動時に落ちるからNULLチェックしてからキャストする
	ULocalPlayer* LocalPlayer = GEngine->GetFirstGamePlayer(World);
	if (LocalPlayer) {
		UYanaiLocalPlayer* local = Cast<UYanaiLocalPlayer>(LocalPlayer);
		if (local) {
			//GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::White, FString::Printf(TEXT("%f"), this->ShiftLens));
			local->SetShiftLens(this->ShiftLens);	// LocalPlayerのシフトレンズを更新
		}
	}
	*/
#endif //0
}
// Fill out your copyright notice in the Description page of Project Settings.


#include "ShiftLensCineCameraComponent.h"

#include "Engine.h"

UShiftLensCineCameraComponent::UShiftLensCineCameraComponent()
	: Super()
{
}

// プレビュー向けの処理
void UShiftLensCineCameraComponent::GetCameraView(float DeltaTime, FMinimalViewInfo& DesiredView)
{
	Super::GetCameraView(DeltaTime, DesiredView);

	// Update shift lens
	DesiredView.OffCenterProjectionOffset.Y = this->ShiftLens;
	UpdateShiftLens();

	// Keep Horizontal
	if (KeepHorizon) {
		DesiredView.Rotation.Pitch = 0.0f;
	}
}		

//
void UShiftLensCineCameraComponent::SetShiftLens(float InShiftLens)
{ 
	ShiftLens = InShiftLens; 
}

// シフトレンズカメラの更新
void UShiftLensCineCameraComponent::UpdateShiftLens()
{
	// エディタからのコール？？？
	AActor* Owner = GetOwner();
	APawn* OwningPawn = Cast<APawn>(GetOwner());
	AController* OwningController = OwningPawn ? OwningPawn->GetController() : nullptr;
	if (OwningController && OwningController->IsLocalPlayerController())
	{
		APlayerController* PlayerController = Cast<APlayerController>(OwningController);
		FLocalPlayerContext Context(PlayerController);
		ULocalPlayer* LocalPlayer = Context.GetLocalPlayer();
		if (LocalPlayer)
		{
//			UShiftLensLocalPlayer* ShiftLensLocalPlayer = Cast<UShiftLensLocalPlayer>(LocalPlayer);
//			if (ShiftLensLocalPlayer)
//			{
//				ShiftLensLocalPlayer->SetShiftLens(this->ShiftLens);
//			}
		}
	}

	// プレイ中のコールはこっち？？？
	UWorld* World = Owner->GetWorld();
	for (FConstPlayerControllerIterator Iterator = World->GetPlayerControllerIterator(); Iterator; ++Iterator)
	{
		APlayerController* PlayerController = Iterator->Get();
		if (PlayerController && PlayerController->PlayerCameraManager)
		{
			//AActor* ViewTarget = PlayerController->GetViewTarget();
			//if (ViewTarget == this) 	// 暫定：ビューポート表示に使用しているカメラだけ処理できる
			//{
			ULocalPlayer* LocalPlayer = PlayerController->GetLocalPlayer();
			if (LocalPlayer)
			{
//				UShiftLensLocalPlayer* ShiftLensLocalPlayer = Cast<UShiftLensLocalPlayer>(LocalPlayer);
//				if (ShiftLensLocalPlayer)
//				{
//					ShiftLensLocalPlayer->SetShiftLens(this->ShiftLens);
//				}
			}
			//}
		}
	}
	
}

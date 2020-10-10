// Fill out your copyright notice in the Description page of Project Settings.


#include "ShiftLensCineCameraComponent.h"

#include "Engine.h"
#include "ShiftLensLocalPlayer.h"

UShiftLensCineCameraComponent::UShiftLensCineCameraComponent()
	: Super()
{
	//this->ShiftLens = 0.0f;
	// カメラコンポーネントの初期位置を得たい...
	//if (ProxyMeshComponent) {
	//	const FTransform ParentWorld = CalcNewComponentToWorld(FTransform());
	//	this->LocationZ = this->ProxyMeshComponent->GetComponentToWorld().GetLocation().Z;
	//	GEngine->AddOnScreenDebugMessage(-1, 1.0f, FColor::Green, FString::Printf(TEXT("222=%f"), 
	//		this->ProxyMeshComponent->GetComponentToWorld().GetLocation().Z));
	//	this->LocationZ = ParentWorld.GetLocation().Z;
	//	GEngine->AddOnScreenDebugMessage(-1, 1.0f, FColor::Green, FString::Printf(TEXT("333=%f"), 
	//		ParentWorld.GetLocation().Z));
	//}
	//this->LocationZ = this->GetComponentLocation().Z;
}

// プレビュー向けの処理
void UShiftLensCineCameraComponent::GetCameraView(float DeltaTime, FMinimalViewInfo& DesiredView)
{
	Super::GetCameraView(DeltaTime, DesiredView);

	//
	DesiredView.OffCenterProjectionOffset.Y = this->ShiftLens;
	UpdateShiftLens();
	//GEngine->AddOnScreenDebugMessage(-1, 1.0f, FColor::Green, FString("GetCameraView"));

	//
	if (KeepHorizon) {
		//FRotator Rotator = DesiredView.Rotation;
		//Rotator.Pitch
		DesiredView.Rotation.Pitch = 0.0f;
		//DesiredView.Location.Z = LocationZ;// this->GetComponentLocation().Z;
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
			UShiftLensLocalPlayer* ShiftLensLocalPlayer = Cast<UShiftLensLocalPlayer>(LocalPlayer);
			if (ShiftLensLocalPlayer)
			{
//				ShiftLensLocalPlayer->SetShiftLens(this->ShiftLens);
			}
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
				UShiftLensLocalPlayer* ShiftLensLocalPlayer = Cast<UShiftLensLocalPlayer>(LocalPlayer);
				if (ShiftLensLocalPlayer)
				{
//					ShiftLensLocalPlayer->SetShiftLens(this->ShiftLens);
				}
			}
			//}
		}
	}
	
}

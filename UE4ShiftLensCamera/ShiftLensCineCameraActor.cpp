// Fill out your copyright notice in the Description page of Project Settings.


#include "ShiftLensCineCameraActor.h"
#include "ShiftLensCineCameraComponent.h"
#include "Engine.h"

//
AShiftLensCineCameraActor::AShiftLensCineCameraActor(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer
			.SetDefaultSubobjectClass<UShiftLensCineCameraComponent>(TEXT("CameraComponent"))
	)
{
	ShiftLensCineCameraComponent = Cast<UShiftLensCineCameraComponent>(GetCineCameraComponent());

	SetActorTickEnabled(true);
	//this->ShiftLens = 0.0f;
}

//
void AShiftLensCineCameraActor::Tick(float DeltaTime)
{
	UpdateShiftLens();
	Super::Tick(DeltaTime);
}

// シフト値を得る(コンポーネントが持っている)
float AShiftLensCineCameraActor::GetShiftLens()
{
	return this->ShiftLensCineCameraComponent->ShiftLens;
}

// シフトレンズカメラの更新
void AShiftLensCineCameraActor::UpdateShiftLens()
{
	this->ShiftLensCineCameraComponent->UpdateShiftLens();
}

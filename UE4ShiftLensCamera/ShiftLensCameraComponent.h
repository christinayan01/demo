// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Camera/CameraComponent.h"
#include "ShiftLensCameraComponent.generated.h"

/**
 * 
 */
UCLASS(HideCategories = (Mobility, Rendering, LOD), Blueprintable, ClassGroup = Camera, meta = (BlueprintSpawnableComponent))
class POLYGON_API UShiftLensCameraComponent : public UCameraComponent
{
	GENERATED_UCLASS_BODY()

public:
	//UFUNCTION(BlueprintCallable, Category = Camera)
	virtual void GetCameraView(float DeltaTime, FMinimalViewInfo& DesiredView) override;

	//
	UPROPERTY(Interp, BlueprintSetter = SetShiftLens, EditAnywhere, BlueprintReadWrite, Category = CameraSettings)
	float ShiftLens;
	UFUNCTION(BlueprintCallable, Category = CameraSettings)
	virtual void SetShiftLens(float InShiftLens) { ShiftLens = InShiftLens; }

};

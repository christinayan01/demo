// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CineCameraActor.h"
//#include "ShiftLensCineCameraComponent.h"
#include "ShiftLensCineCameraActor.generated.h"

/**
 * 
 */
UCLASS(ClassGroup = Common, hideCategories = (Input, Rendering, AutoPlayerActivation), showcategories = ("Input|MouseInput", "Input|TouchInput"), Blueprintable)
class SHIFTLENS1_API AShiftLensCineCameraActor : public ACineCameraActor
{
	GENERATED_BODY()

public:
	AShiftLensCineCameraActor(const FObjectInitializer& ObjectInitializer);

	virtual void Tick(float DeltaTime) override;

	// シフト値を得る(コンポーネントが持っている)
	UFUNCTION(BlueprintCallable, Category = "Cine Camera")
	float GetShiftLens();

	// シフトレンズカメラの更新
	UFUNCTION(BlueprintCallable, Category = "Cine Camera")
	void UpdateShiftLens();

	/** Returns the CineCameraComponent of this CineCamera */
	UFUNCTION(BlueprintCallable, Category = "Cine Camera")
	UShiftLensCineCameraComponent* GetShiftLensCineCameraComponent() const { return ShiftLensCineCameraComponent; }

private:
	/** Returns CineCameraComponent subobject **/
	class UShiftLensCineCameraComponent* ShiftLensCineCameraComponent;


};

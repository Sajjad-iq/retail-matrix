import * as React from "react"
import { cn } from "@/lib/utils"
import { Check } from "lucide-react"

type StepState = "active" | "completed" | "inactive"

interface StepperContextValue {
  currentStep: number
  totalSteps: number
  orientation?: "horizontal" | "vertical"
}

const StepperContext = React.createContext<StepperContextValue | undefined>(
  undefined
)

function useStepper() {
  const context = React.useContext(StepperContext)
  if (!context) {
    throw new Error("Stepper components must be used within a Stepper")
  }
  return context
}

interface StepperProps extends React.HTMLAttributes<HTMLDivElement> {
  currentStep: number
  totalSteps?: number
  orientation?: "horizontal" | "vertical"
}

const Stepper = React.forwardRef<HTMLDivElement, StepperProps>(
  ({ className, currentStep, totalSteps, orientation = "horizontal", children, ...props }, ref) => {
    // If totalSteps is not provided, count the children
    const childCount = React.Children.count(children)
    const steps = totalSteps || childCount

    return (
      <StepperContext.Provider value={{ currentStep, totalSteps: steps, orientation }}>
        <div
          ref={ref}
          className={cn(
            "flex",
            orientation === "vertical" ? "flex-col gap-0" : "gap-4 items-start justify-between w-full",
            className
          )}
          {...props}
        >
          {children}
        </div>
      </StepperContext.Provider>
    )
  }
)
Stepper.displayName = "Stepper"

interface StepperItemContextValue {
  step: number
  state: StepState
  disabled?: boolean
}

const StepperItemContext = React.createContext<StepperItemContextValue | undefined>(
  undefined
)

function useStepperItem() {
  const context = React.useContext(StepperItemContext)
  if (!context) {
    throw new Error("StepperItem components must be used within a StepperItem")
  }
  return context
}

interface StepperItemProps extends React.HTMLAttributes<HTMLDivElement> {
  step: number
  disabled?: boolean
}

const StepperItem = React.forwardRef<HTMLDivElement, StepperItemProps>(
  ({ className, step, disabled, children, ...props }, ref) => {
    const { currentStep } = useStepper()

    let state: StepState
    if (step < currentStep) {
      state = "completed"
    } else if (step === currentStep) {
      state = "active"
    } else {
      state = "inactive"
    }

    return (
      <StepperItemContext.Provider value={{ step, state, disabled }}>
        <div
          ref={ref}
          data-state={state}
          data-disabled={disabled ? "" : undefined}
          className={cn(
            "flex items-start gap-4 group flex-1 relative",
            disabled && "pointer-events-none opacity-50",
            className
          )}
          {...props}
        >
          {children}
        </div>
      </StepperItemContext.Provider>
    )
  }
)
StepperItem.displayName = "StepperItem"

interface StepperTriggerProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  asChild?: boolean
}

const StepperTrigger = React.forwardRef<HTMLButtonElement, StepperTriggerProps>(
  ({ className, asChild, children, ...props }, ref) => {
    const Comp = asChild ? React.Fragment : "div"
    const childProps = asChild ? {} : props

    if (asChild && React.isValidElement(children)) {
      return React.cloneElement(children, {
        ref,
        className: cn(
          "flex items-start gap-4 w-full",
          className
        ),
        ...props,
      } as any)
    }

    return (
      <Comp
        ref={asChild ? undefined : ref}
        className={cn(
          "flex items-start gap-4 w-full",
          className
        )}
        {...(childProps as any)}
      >
        {children}
      </Comp>
    )
  }
)
StepperTrigger.displayName = "StepperTrigger"

interface StepperIndicatorProps extends React.HTMLAttributes<HTMLDivElement> {}

const StepperIndicator = React.forwardRef<HTMLDivElement, StepperIndicatorProps>(
  ({ className, children, ...props }, ref) => {
    const { step, state } = useStepperItem()

    return (
      <div className="relative flex items-center justify-center shrink-0">
        <div
          ref={ref}
          data-state={state}
          className={cn(
            "relative z-10 inline-flex items-center justify-center rounded-full w-12 h-12 text-base font-bold transition-all duration-300 shrink-0",
            // Inactive
            state === "inactive" && "bg-muted/50 text-muted-foreground border-2 border-border",
            // Active
            state === "active" && "bg-primary text-primary-foreground shadow-lg ring-4 ring-primary/30",
            // Completed
            state === "completed" && "bg-primary text-primary-foreground shadow-md",
            className
          )}
          {...props}
        >
          {children || (state === "completed" ? <Check className="h-6 w-6 stroke-[3]" /> : step)}
        </div>
        {/* Pulse effect for active state */}
        {state === "active" && (
          <div className="absolute inset-0 rounded-full bg-primary/20 animate-ping" />
        )}
      </div>
    )
  }
)
StepperIndicator.displayName = "StepperIndicator"

interface StepperSeparatorProps extends React.HTMLAttributes<HTMLDivElement> {}

const StepperSeparator = React.forwardRef<HTMLDivElement, StepperSeparatorProps>(
  ({ className, ...props }, ref) => {
    const { state } = useStepperItem()
    const { orientation } = useStepper()

    return (
      <div
        ref={ref}
        data-state={state}
        className={cn(
          "transition-all duration-500 mx-2",
          orientation === "vertical" ? "h-8 w-0.5 ml-4" : "h-0.5 flex-1 self-center",
          // Default
          "bg-border",
          // Completed
          state === "completed" && "bg-primary",
          // Active
          state === "active" && "bg-primary/50",
          className
        )}
        {...props}
      />
    )
  }
)
StepperSeparator.displayName = "StepperSeparator"

interface StepperTitleProps extends React.HTMLAttributes<HTMLDivElement> {}

const StepperTitle = React.forwardRef<HTMLDivElement, StepperTitleProps>(
  ({ className, ...props }, ref) => {
    const { state } = useStepperItem()

    return (
      <div
        ref={ref}
        className={cn(
          "text-base font-bold transition-all duration-300 leading-tight",
          state === "active" && "text-primary",
          state === "completed" && "text-foreground",
          state === "inactive" && "text-muted-foreground",
          className
        )}
        {...props}
      />
    )
  }
)
StepperTitle.displayName = "StepperTitle"

interface StepperDescriptionProps extends React.HTMLAttributes<HTMLDivElement> {}

const StepperDescription = React.forwardRef<HTMLDivElement, StepperDescriptionProps>(
  ({ className, ...props }, ref) => {
    const { state } = useStepperItem()

    return (
      <div
        ref={ref}
        className={cn(
          "text-sm transition-all duration-300 leading-relaxed mt-0.5",
          state === "active" && "text-muted-foreground",
          state === "completed" && "text-muted-foreground/80",
          state === "inactive" && "text-muted-foreground/50",
          className
        )}
        {...props}
      />
    )
  }
)
StepperDescription.displayName = "StepperDescription"

export {
  Stepper,
  StepperItem,
  StepperTrigger,
  StepperIndicator,
  StepperSeparator,
  StepperTitle,
  StepperDescription,
}

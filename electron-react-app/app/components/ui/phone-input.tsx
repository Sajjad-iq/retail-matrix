import * as React from "react"
import { Input } from "@/app/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/app/components/ui/select"
import { cn } from "@/lib/utils"

// Common country codes with their dial codes and flags
const COUNTRIES = [
  { code: "IQ", name: "Iraq", dialCode: "+964", flag: "🇮🇶" },
  { code: "US", name: "United States", dialCode: "+1", flag: "🇺🇸" },
  { code: "GB", name: "United Kingdom", dialCode: "+44", flag: "🇬🇧" },
  { code: "CA", name: "Canada", dialCode: "+1", flag: "🇨🇦" },
  { code: "AU", name: "Australia", dialCode: "+61", flag: "🇦🇺" },
  { code: "DE", name: "Germany", dialCode: "+49", flag: "🇩🇪" },
  { code: "FR", name: "France", dialCode: "+33", flag: "🇫🇷" },
  { code: "IT", name: "Italy", dialCode: "+39", flag: "🇮🇹" },
  { code: "ES", name: "Spain", dialCode: "+34", flag: "🇪🇸" },
  { code: "JP", name: "Japan", dialCode: "+81", flag: "🇯🇵" },
  { code: "CN", name: "China", dialCode: "+86", flag: "🇨🇳" },
  { code: "IN", name: "India", dialCode: "+91", flag: "🇮🇳" },
  { code: "BR", name: "Brazil", dialCode: "+55", flag: "🇧🇷" },
  { code: "MX", name: "Mexico", dialCode: "+52", flag: "🇲🇽" },
  { code: "RU", name: "Russia", dialCode: "+7", flag: "🇷🇺" },
  { code: "ZA", name: "South Africa", dialCode: "+27", flag: "🇿🇦" },
  { code: "KR", name: "South Korea", dialCode: "+82", flag: "🇰🇷" },
  { code: "AR", name: "Argentina", dialCode: "+54", flag: "🇦🇷" },
  { code: "EG", name: "Egypt", dialCode: "+20", flag: "🇪🇬" },
  { code: "SA", name: "Saudi Arabia", dialCode: "+966", flag: "🇸🇦" },
  { code: "AE", name: "United Arab Emirates", dialCode: "+971", flag: "🇦🇪" },
  { code: "TR", name: "Turkey", dialCode: "+90", flag: "🇹🇷" },
  { code: "NL", name: "Netherlands", dialCode: "+31", flag: "🇳🇱" },
  { code: "SE", name: "Sweden", dialCode: "+46", flag: "🇸🇪" },
  { code: "NO", name: "Norway", dialCode: "+47", flag: "🇳🇴" },
  { code: "DK", name: "Denmark", dialCode: "+45", flag: "🇩🇰" },
  { code: "PL", name: "Poland", dialCode: "+48", flag: "🇵🇱" },
  { code: "BE", name: "Belgium", dialCode: "+32", flag: "🇧🇪" },
  { code: "AT", name: "Austria", dialCode: "+43", flag: "🇦🇹" },
  { code: "CH", name: "Switzerland", dialCode: "+41", flag: "🇨🇭" },
  { code: "PT", name: "Portugal", dialCode: "+351", flag: "🇵🇹" },
  { code: "GR", name: "Greece", dialCode: "+30", flag: "🇬🇷" },
  { code: "CZ", name: "Czech Republic", dialCode: "+420", flag: "🇨🇿" },
  { code: "RO", name: "Romania", dialCode: "+40", flag: "🇷🇴" },
  { code: "HU", name: "Hungary", dialCode: "+36", flag: "🇭🇺" },
  { code: "IL", name: "Israel", dialCode: "+972", flag: "🇮🇱" },
  { code: "SG", name: "Singapore", dialCode: "+65", flag: "🇸🇬" },
  { code: "MY", name: "Malaysia", dialCode: "+60", flag: "🇲🇾" },
  { code: "TH", name: "Thailand", dialCode: "+66", flag: "🇹🇭" },
  { code: "PH", name: "Philippines", dialCode: "+63", flag: "🇵🇭" },
  { code: "ID", name: "Indonesia", dialCode: "+62", flag: "🇮🇩" },
  { code: "VN", name: "Vietnam", dialCode: "+84", flag: "🇻🇳" },
  { code: "NZ", name: "New Zealand", dialCode: "+64", flag: "🇳🇿" },
  { code: "PK", name: "Pakistan", dialCode: "+92", flag: "🇵🇰" },
  { code: "BD", name: "Bangladesh", dialCode: "+880", flag: "🇧🇩" },
  { code: "NG", name: "Nigeria", dialCode: "+234", flag: "🇳🇬" },
  { code: "KE", name: "Kenya", dialCode: "+254", flag: "🇰🇪" },
  { code: "GH", name: "Ghana", dialCode: "+233", flag: "🇬🇭" },
]

export interface PhoneInputProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, "onChange" | "value"> {
  /** Combined phone value (e.g., "+964 7501234567") */
  value?: string
  defaultCountry?: string
  /** Called with combined phone number (code + number) */
  onChange?: (value: string) => void
}

/**
 * Parse a combined phone value into country code and local number
 */
function parsePhoneValue(value: string, defaultCountry: string): { countryCode: string; localNumber: string } {
  if (!value) {
    const country = COUNTRIES.find(c => c.code === defaultCountry) || COUNTRIES[0]
    return { countryCode: country.code, localNumber: "" }
  }

  // Try to match the dial code at the start
  const trimmed = value.trim()
  for (const country of COUNTRIES) {
    if (trimmed.startsWith(country.dialCode)) {
      const localNumber = trimmed.slice(country.dialCode.length).trim()
      return { countryCode: country.code, localNumber }
    }
  }

  // No match found, use default country
  const country = COUNTRIES.find(c => c.code === defaultCountry) || COUNTRIES[0]
  return { countryCode: country.code, localNumber: trimmed.replace(/^\+\d+\s*/, "") }
}

const PhoneInput = React.forwardRef<HTMLInputElement, PhoneInputProps>(
  (
    {
      className,
      value = "",
      defaultCountry = "IQ",
      disabled,
      placeholder = "Enter your phone number",
      onChange,
      ...props
    },
    ref
  ) => {
    // Parse the combined value into parts
    const { countryCode: initialCountryCode, localNumber: initialLocalNumber } = React.useMemo(
      () => parsePhoneValue(value, defaultCountry),
      [value, defaultCountry]
    )

    const [selectedCountry, setSelectedCountry] = React.useState(() => {
      return COUNTRIES.find(c => c.code === initialCountryCode) || COUNTRIES[0]
    })
    const [phoneNumber, setPhoneNumber] = React.useState(initialLocalNumber)

    // Update state when value prop changes externally
    React.useEffect(() => {
      const { countryCode, localNumber } = parsePhoneValue(value, defaultCountry)
      const country = COUNTRIES.find(c => c.code === countryCode)
      if (country) {
        setSelectedCountry(country)
      }
      setPhoneNumber(localNumber)
    }, [value, defaultCountry])

    // Basic validation - checks if phone number has at least 6 digits
    const isValid = React.useMemo(() => {
      const digitsOnly = phoneNumber.replace(/\D/g, "")
      return digitsOnly.length >= 6
    }, [phoneNumber])

    // Combine and emit the full phone number
    const emitCombinedValue = React.useCallback((dialCode: string, number: string) => {
      const combined = number ? `${dialCode} ${number}`.trim() : ""
      onChange?.(combined)
    }, [onChange])

    const handleCountryChange = (countryCode: string) => {
      const country = COUNTRIES.find((c) => c.code === countryCode)
      if (country) {
        setSelectedCountry(country)
        emitCombinedValue(country.dialCode, phoneNumber)
      }
    }

    const handlePhoneNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      const newValue = e.target.value
      // Only allow numbers, spaces, hyphens, and parentheses
      const sanitized = newValue.replace(/[^\d\s\-()]/g, "")
      setPhoneNumber(sanitized)
      emitCombinedValue(selectedCountry.dialCode, sanitized)
    }

    return (
      <div className={cn("flex gap-2", className)}>
        <Select
          value={selectedCountry.code}
          onValueChange={handleCountryChange}
          disabled={disabled}
        >
          <SelectTrigger className="w-[140px]">
            <SelectValue>
              <div className="flex items-center gap-2">
                <span>{selectedCountry.flag}</span>
                <span className="text-sm">{selectedCountry.dialCode}</span>
              </div>
            </SelectValue>
          </SelectTrigger>
          <SelectContent className="max-h-[300px]">
            {COUNTRIES.map((country) => (
              <SelectItem key={country.code} value={country.code}>
                <div className="flex items-center gap-2">
                  <span>{country.flag}</span>
                  <span className="text-sm">{country.name}</span>
                  <span className="text-xs text-muted-foreground">
                    {country.dialCode}
                  </span>
                </div>
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
        <Input
          ref={ref}
          type="tel"
          value={phoneNumber}
          onChange={handlePhoneNumberChange}
          placeholder={placeholder}
          disabled={disabled}
          className={cn("flex-1", !isValid && phoneNumber && "border-destructive")}
          {...props}
        />
      </div>
    )
  }
)
PhoneInput.displayName = "PhoneInput"

export { PhoneInput }

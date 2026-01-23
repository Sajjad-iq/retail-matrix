"use client"

import * as React from "react"
import { X, Image, Video, FileText, Folder, FileType, Archive } from "lucide-react"
import { useMutation } from "@tanstack/react-query"
import { toast } from "sonner"
import { Button } from "@/app/components/ui/button"
import { cn } from "@/lib/utils"

// Media categories - generic file types
export enum MediaCategory {
  IMAGE = 'IMAGE',
  VIDEO = 'VIDEO',
  DOCUMENT = 'DOCUMENT',
  PDF = 'PDF',
  FOLDER = 'FOLDER',
  ARCHIVE = 'ARCHIVE',
}

// Category-specific configurations
const CATEGORY_CONFIG = {
  [MediaCategory.IMAGE]: {
    icon: Image,
    accept: "image/*",
    maxSize: 5, // 5MB for images
    label: "image",
  },
  [MediaCategory.VIDEO]: {
    icon: Video,
    accept: "video/*",
    maxSize: 50, // 50MB for videos
    label: "video",
  },
  [MediaCategory.DOCUMENT]: {
    icon: FileText,
    accept: ".doc,.docx,.txt,.rtf",
    maxSize: 10, // 10MB for documents
    label: "document",
  },
  [MediaCategory.PDF]: {
    icon: FileType,
    accept: "application/pdf",
    maxSize: 10, // 10MB for PDFs
    label: "PDF",
  },
  [MediaCategory.FOLDER]: {
    icon: Folder,
    accept: "*/*", // Accept all files for folder/general uploads
    maxSize: 20, // 20MB for general files
    label: "file",
  },
  [MediaCategory.ARCHIVE]: {
    icon: Archive,
    accept: ".zip,.rar,.7z,.tar,.gz",
    maxSize: 100, // 100MB for archives
    label: "archive",
  },
} as const

export interface ImageUploadProps {
  value?: string
  onChange: (value: string) => void
  category?: MediaCategory
  maxSize?: number
  previewAlt?: string
  disabled?: boolean
  onUpload?: (
    file: File,
    category: MediaCategory,
  ) => Promise<{ success: boolean; data?: { url: string }; error?: string }>
  className?: string
}

export function ImageUpload({
  value,
  onChange,
  category = MediaCategory.IMAGE,
  maxSize: customMaxSize,
  previewAlt = "Image preview",
  disabled = false,
  onUpload,
  className,
}: ImageUploadProps) {
  const inputRef = React.useRef<HTMLInputElement>(null)

  // Get category-specific config
  const config = CATEGORY_CONFIG[category]
  const maxSize = customMaxSize ?? config.maxSize
  const Icon = config.icon

  // Upload mutation
  const uploadMutation = useMutation({
    mutationFn: async (file: File) => {
      if (onUpload) {
        const result = await onUpload(file, category)
        if (result.success && result.data?.url) {
          return result.data.url
        } else {
          throw new Error(result.error || "Upload failed")
        }
      } else {
        // Fallback: create a local URL for preview
        return URL.createObjectURL(file)
      }
    },
    onSuccess: (url) => {
      onChange(url)
      toast.success(`${config.label.charAt(0).toUpperCase() + config.label.slice(1)} uploaded successfully`)
    },
    onError: (error: Error) => {
      toast.error(error.message || `Failed to upload ${config.label}`)
    },
  })

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return

    // Validate file size
    const fileSizeMB = file.size / (1024 * 1024)
    if (fileSizeMB > maxSize) {
      toast.error(`File size must be less than ${maxSize}MB`)
      return
    }

    // Validate file type based on category
    const acceptedTypes = config.accept.split(',').map(t => t.trim())
    const isValidType = acceptedTypes.some(type => {
      if (type === "image/*") return file.type.startsWith("image/")
      if (type === "application/pdf") return file.type === "application/pdf"
      if (type.startsWith(".")) return file.name.toLowerCase().endsWith(type)
      return file.type === type
    })

    if (!isValidType) {
      toast.error(`Please upload a valid ${config.label}`)
      return
    }

    uploadMutation.mutate(file)
  }

  const handleRemove = () => {
    onChange("")
    if (inputRef.current) {
      inputRef.current.value = ""
    }
  }

  return (
    <div className={cn("space-y-2", className)}>
      <input
        ref={inputRef}
        type="file"
        accept={config.accept}
        onChange={handleFileChange}
        disabled={disabled || uploadMutation.isPending}
        className="hidden"
      />

      {value ? (
        <div className="relative inline-block">
          <img
            src={value}
            alt={previewAlt}
            className="max-w-[200px] max-h-[200px] rounded-lg border object-cover"
          />
          {!disabled && (
            <Button
              type="button"
              variant="destructive"
              size="icon"
              className="absolute -top-2 -right-2 h-6 w-6"
              onClick={handleRemove}
            >
              <X className="h-4 w-4" />
            </Button>
          )}
        </div>
      ) : (
        <Button
          type="button"
          variant="outline"
          disabled={disabled || uploadMutation.isPending}
          onClick={() => inputRef.current?.click()}
          className="h-32 w-full border-dashed"
        >
          <div className="flex flex-col items-center gap-2 text-muted-foreground">
            {uploadMutation.isPending ? (
              <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-primary" />
            ) : (
              <>
                <Icon className="h-8 w-8" />
                <span className="text-sm">Click to upload {config.label}</span>
                <span className="text-xs">Max size: {maxSize}MB</span>
              </>
            )}
          </div>
        </Button>
      )}
    </div>
  )
}

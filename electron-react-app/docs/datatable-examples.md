# DataTable Real-World Examples

> Examples extracted from `app/(app)/(admin)/pre-registration-management/`

## Example 1: Basic Table with Server Pagination

**Source**: `page.tsx`

```typescript
'use client';

import { useMemo } from 'react';
import { DataTable } from '@/components/dataTable';
import { createDefaultTableColumns } from './config/default-table-config';
import useData from './useData';
import type { FamilyGroup } from './types';

export default function PreRegistrationManagementPage() {
  const {
    families,
    loading,
    currentPage,
    setCurrentPage,
    pageSize,
    setPageSize,
    totalCount,
    totalPages,
    fetchRecords,
  } = useData();
  
  // Create columns with callbacks
  const columns = useMemo(
    () => createDefaultTableColumns(),
    []
  );
  
  // Pagination state for DataTable
  const pagination = useMemo(
    () => ({
      page: currentPage - 1, // DataTable uses 0-based indexing
      size: pageSize,
      totalElements: totalCount,
      totalPages: totalPages,
      onPageChange: (page: number) => setCurrentPage(page + 1),
      onPageSizeChange: setPageSize,
    }),
    [currentPage, pageSize, totalCount, totalPages]
  );
  
  return (
    <div className="h-[650px]">
      <DataTable
        key={currentPage}
        data={families}
        columns={columns}
        isLoading={loading}
        onRetry={fetchRecords}
        pagination={pagination}
        showToolbar={false}
        paginationConfig={{
          showPageInfo: true,
          showSelectedCount: false,
          showPageSize: false,
          showFirstLastButtons: false,
          showPrevNextButtons: true,
          showPageInput: false,
        }}
      />
    </div>
  );
}
```

**Key Patterns**:
- ✅ Server-side pagination with 0-based conversion
- ✅ useMemo for columns and pagination
- ✅ Custom pagination configuration
- ✅ Retry handler for error recovery
- ✅ Fixed height container
- ✅ Key prop for forcing re-render on page change

---

## Example 2: Table Columns with Actions & Custom Rendering

**Source**: `config/default-table-config.tsx`

```typescript
'use client';

import { ColumnDef } from '@tanstack/react-table';
import {
  ChevronDown,
  ChevronUp,
  Phone,
  CreditCard,
  UserCircle,
  CheckCircle,
  XCircle,
  Printer,
  Loader2,
} from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import type { FamilyGroup, FamilyMember } from '../types';

interface ColumnsOptions {
  onPrintCard?: (member: FamilyMember) => void;
  onRowExpand?: (row: FamilyGroup) => void;
  loadingFamilyMembers?: Set<string>;
}

export const createDefaultTableColumns = (
  options: ColumnsOptions = {},
): ColumnDef<FamilyGroup>[] => [
  {
    id: 'expander',
    header: () => null,
    cell: ({ row }) => {
      const familyHeadId = row.original.familyHead.id.toString();
      const isLoading = options.loadingFamilyMembers?.has(familyHeadId);

      return (
        <div className="flex items-center justify-center">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => {
              if (!row.getIsExpanded() && options.onRowExpand) {
                options.onRowExpand(row.original);
              }
              row.toggleExpanded();
            }}
            disabled={isLoading}
            className="h-6 w-6 p-0"
          >
            {isLoading ? (
              <Loader2 className="h-4 w-4 animate-spin" />
            ) : row.getIsExpanded() ? (
              <ChevronUp className="h-4 w-4" />
            ) : (
              <ChevronDown className="h-4 w-4" />
            )}
          </Button>
        </div>
      );
    },
  },
  {
    id: 'name',
    accessorFn: (row) => row.familyHead.fullName,
    header: 'رب الأسرة / أفراد الأسرة',
    cell: ({ row }) => {
      const family = row.original;

      return (
        <div className="flex items-center justify-center gap-2">
          <UserCircle className="h-5 w-5 text-blue-700" />
          <span className="font-bold text-blue-900">
            {family.familyHead.fullName}
          </span>
          <Badge variant="default" className="bg-blue-700">
            {family.familyHead.relationshipType || 'رب الأسرة'}
          </Badge>
          {family.totalMembers > 0 && (
            <Badge variant="secondary">{family.totalMembers} أفراد</Badge>
          )}
        </div>
      );
    },
  },
  {
    id: 'organization',
    accessorFn: (row) => row.familyHead.organizationName,
    header: 'المؤسسة',
    cell: ({ getValue }) => (
      <span className="text-sm">{(getValue() as string) || '-'}</span>
    ),
  },
  {
    id: 'subOrganization',
    accessorFn: (row) => row.familyHead.subOrganizationName,
    header: 'المؤسسة الفرعية',
    cell: ({ getValue }) => (
      <span className="text-sm">{(getValue() as string) || '-'}</span>
    ),
  },
  {
    id: 'phone',
    accessorFn: (row) => row.familyHead.phoneNumber,
    header: 'رقم الهاتف',
    cell: ({ getValue }) => (
      <div className="flex items-center justify-center gap-1">
        <Phone className="text-muted-foreground h-3 w-3" />
        {(getValue() as string) || '-'}
      </div>
    ),
  },
  {
    id: 'nationalNumber',
    accessorFn: (row) =>
      row.familyHead.nationalNumber || row.familyHead.civilId,
    header: 'الرقم الوطني / الهوية',
    cell: ({ getValue }) => (
      <div className="text-xs">{(getValue() as string) || '-'}</div>
    ),
  },
  {
    id: 'insuranceCardId',
    accessorFn: (row) => row.familyHead.insuranceCardId,
    header: 'رقم البطاقة التأمينية',
    cell: ({ getValue }) => (
      <div className="flex items-center justify-center gap-1">
        <CreditCard className="text-muted-foreground h-3 w-3" />
        {(getValue() as string) || '-'}
      </div>
    ),
  },
  {
    id: 'gender',
    accessorFn: (row) => row.familyHead.gender,
    header: 'الجنس',
    cell: ({ getValue }) => {
      const gender = (getValue() as string)?.toLowerCase() || '';
      if (gender === 'male' || gender === 'ذكر') {
        return (
          <Badge
            variant="outline"
            className="border-blue-200 bg-blue-50 text-blue-700"
          >
            ذكر
          </Badge>
        );
      }
      if (gender === 'female' || gender === 'أنثى' || gender === 'انثى') {
        return (
          <Badge
            variant="outline"
            className="border-pink-200 bg-pink-50 text-pink-700"
          >
            أنثى
          </Badge>
        );
      }
      return <span>{(getValue() as string) || '-'}</span>;
    },
  },
  {
    id: 'status',
    accessorFn: (row) => row.familyHead.hasCompletedProfile,
    header: 'الحالة',
    cell: ({ getValue }) => {
      const isComplete = getValue() as boolean;
      return isComplete ? (
        <Badge
          variant="outline"
          className="gap-1 border-green-200 bg-green-50 text-green-700"
        >
          <CheckCircle className="h-3 w-3" />
          مكتمل
        </Badge>
      ) : (
        <Badge
          variant="outline"
          className="gap-1 border-yellow-200 bg-yellow-50 text-yellow-700"
        >
          <XCircle className="h-3 w-3" />
          غير مكتمل
        </Badge>
      );
    },
  },
  {
    id: 'actions',
    header: 'الإجراءات',
    cell: ({ row }) => {
      const family = row.original;
      return family.familyHead.insuranceCardId ? (
        <Button
          variant="outline"
          size="sm"
          onClick={() => options.onPrintCard?.(family.familyHead)}
          title="طباعة البطاقة"
          className="gap-1"
        >
          <Printer className="h-4 w-4" />
          طباعة
        </Button>
      ) : (
        <span className="text-muted-foreground text-xs">لا توجد بطاقة</span>
      );
    },
  },
];
```

**Key Patterns**:
- ✅ Options object for callbacks
- ✅ Expander column with loading state
- ✅ accessorFn for computed values
- ✅ Rich cell rendering with icons and badges
- ✅ Conditional rendering (gender, status)
- ✅ Multiple badges in single cell
- ✅ Fallback values with null handling
- ✅ Conditional actions based on data
- ✅ TypeScript types for type safety

---

## Example 3: Expandable Rows with Sub-Table

**Source**: `components/family-member-table.tsx`

```typescript
'use client';

import * as React from 'react';
import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
} from '@tanstack/react-table';
import { Loader2 } from 'lucide-react';
import { TableRow, TableCell } from '@/components/ui/table';
import type { FamilyGroup, FamilyMember } from '../types';
import { createFamilyMemberTableColumns } from '../config/family-member-table-config';

interface RenderSubRowOptions {
  onPrintCard?: (member: FamilyMember) => void;
  loadingFamilyMembers: Set<string>;
}

interface FamilyMemberSubRowsProps {
  family: FamilyGroup;
  columns: ColumnDef<FamilyMember>[];
  isLoading: boolean;
}

/**
 * Internal component for rendering family member sub-rows
 */
function FamilyMemberSubRows({
  family,
  columns,
  isLoading,
}: FamilyMemberSubRowsProps) {
  const subTable = useReactTable({
    data: family.familyMembers,
    columns,
    getCoreRowModel: getCoreRowModel(),
  });

  const subRows = subTable.getRowModel().rows;

  if (isLoading) {
    return (
      <TableRow>
        <TableCell colSpan={columns.length} className="py-4 text-center">
          <div className="flex items-center justify-center gap-2">
            <Loader2 className="h-4 w-4 animate-spin" />
            <span className="text-muted-foreground text-sm">
              جارٍ تحميل أفراد الأسرة...
            </span>
          </div>
        </TableCell>
      </TableRow>
    );
  }

  if (subRows.length === 0) {
    return (
      <TableRow>
        <TableCell colSpan={columns.length} className="py-4 text-center">
          <span className="text-muted-foreground text-sm">
            لا يوجد أفراد في هذه الأسرة
          </span>
        </TableCell>
      </TableRow>
    );
  }

  return (
    <>
      {subRows.map((row) => (
        <TableRow
          key={row.id}
          className="border-gray-200 transition-colors hover:bg-gray-50"
        >
          {row.getVisibleCells().map((cell) => (
            <TableCell key={cell.id} className="text-center whitespace-nowrap">
              {flexRender(cell.column.columnDef.cell, cell.getContext())}
            </TableCell>
          ))}
        </TableRow>
      ))}
    </>
  );
}

/**
 * Creates a renderSubRow function for family members.
 * This is the main export - handles columns and rendering internally.
 */
export const createRenderSubRow = (options: RenderSubRowOptions) => {
  const columns = createFamilyMemberTableColumns({
    onPrintCard: options.onPrintCard,
  });

  const RenderSubRow = (family: FamilyGroup) => {
    const isLoading = options.loadingFamilyMembers.has(
      family.familyHead.id.toString(),
    );
    return (
      <FamilyMemberSubRows
        family={family}
        columns={columns}
        isLoading={isLoading}
      />
    );
  };
  RenderSubRow.displayName = 'RenderSubRow';
  return RenderSubRow;
};
```

**Usage in page.tsx**:
```typescript
// Create table meta with expansion config
const tableMeta = useMemo(
  () => ({
    ...createTableCustomStyle(),
    renderSubRow: createRenderSubRow({
      onPrintCard: handlePrintCard,
      loadingFamilyMembers,
    }),
  }),
  [handlePrintCard, loadingFamilyMembers],
);

<DataTable
  data={families}
  columns={columns}
  meta={tableMeta}
  isLoading={loading}
  pagination={pagination}
/>
```

**Key Patterns**:
- ✅ Separate sub-row component
- ✅ useReactTable for sub-table
- ✅ Loading state for async sub-row data
- ✅ Empty state handling
- ✅ Factory function pattern (createRenderSubRow)
- ✅ Display name for React DevTools
- ✅ Proper TypeScript types
- ✅ Columns created internally

---

## Example 4: Custom Table Styling

**Source**: `config/default-table-config.tsx`

```typescript
/**
 * Creates table meta configuration with styling
 */
export const createTableCustomStyle = () => ({
  tableHeaderClassName: 'bg-blue-50 border-border sticky top-0 z-10 border-b-2',
  tableHeaderRowClassName: 'border-0 hover:bg-blue-50',
  tableHeaderCellClassName:
    'text-center font-semibold whitespace-nowrap text-blue-900',
  tableBodyRowClassName:
    'border-b-2 border-blue-300 bg-blue-100 font-semibold hover:bg-blue-200',
  tableBodyCellClassName: 'text-center whitespace-nowrap',
});

// Usage
const tableMeta = useMemo(
  () => ({
    ...createTableCustomStyle(),
    renderSubRow: createRenderSubRow({ ... }),
  }),
  []
);
```

**Key Patterns**:
- ✅ Factory function for reusability
- ✅ Sticky header with z-index
- ✅ Custom colors and borders
- ✅ Consistent alignment
- ✅ Hover states
- ✅ Whitespace control

---

## Example 5: Family Member Sub-Table Columns

**Source**: `config/family-member-table-config.tsx`

```typescript
'use client';

import { ColumnDef } from '@tanstack/react-table';
import { Users, CheckCircle, XCircle, Printer } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import type { FamilyMember } from '../types';

interface ColumnsOptions {
  onPrintCard?: (member: FamilyMember) => void;
}

/**
 * Column definitions for family members table
 */
export const createFamilyMemberTableColumns = (
  options: ColumnsOptions = {},
): ColumnDef<FamilyMember>[] => [
  {
    id: 'expander',
    header: () => null,
    cell: () => null,  // Empty cell to maintain column alignment
  },
  {
    id: 'name',
    accessorKey: 'fullName',
    header: 'رب الأسرة / أفراد الأسرة',
    cell: ({ row }) => {
      const member = row.original;
      return (
        <div className="flex items-center justify-center gap-2 pr-8">
          <Users className="h-4 w-4 text-gray-500" />
          <span className="text-gray-700">{member.fullName}</span>
          {member.relationshipType && (
            <Badge variant="outline" className="text-xs">
              {member.relationshipType}
            </Badge>
          )}
        </div>
      );
    },
  },
  {
    id: 'organization',
    accessorKey: 'organizationName',
    header: 'المؤسسة',
    cell: ({ getValue }) => (
      <span className="text-xs text-gray-600">
        {(getValue() as string) || '-'}
      </span>
    ),
  },
  {
    id: 'subOrganization',
    accessorKey: 'subOrganizationName',
    header: 'المؤسسة الفرعية',
    cell: ({ getValue }) => (
      <span className="text-xs text-gray-600">
        {(getValue() as string) || '-'}
      </span>
    ),
  },
  {
    id: 'phone',
    accessorKey: 'phoneNumber',
    header: 'رقم الهاتف',
    cell: ({ getValue }) => (
      <span className="text-xs">{(getValue() as string) || '-'}</span>
    ),
  },
  {
    id: 'nationalNumber',
    accessorFn: (row) => row.nationalNumber || row.civilId,
    header: 'الرقم الوطني / الهوية',
    cell: ({ getValue }) => (
      <span className="text-xs">{(getValue() as string) || '-'}</span>
    ),
  },
  {
    id: 'insuranceCardId',
    accessorKey: 'insuranceCardId',
    header: 'رقم البطاقة التأمينية',
    cell: ({ getValue }) => (
      <span className="text-xs">{(getValue() as string) || '-'}</span>
    ),
  },
  {
    id: 'gender',
    accessorKey: 'gender',
    header: 'الجنس',
    cell: ({ getValue }) => {
      const gender = (getValue() as string)?.toLowerCase() || '';
      if (gender === 'male' || gender === 'ذكر') {
        return (
          <Badge
            variant="outline"
            className="border-blue-200 bg-blue-50 text-xs text-blue-700"
          >
            ذكر
          </Badge>
        );
      }
      if (gender === 'female' || gender === 'أنثى' || gender === 'انثى') {
        return (
          <Badge
            variant="outline"
            className="border-pink-200 bg-pink-50 text-xs text-pink-700"
          >
            أنثى
          </Badge>
        );
      }
      return <span className="text-xs">{(getValue() as string) || '-'}</span>;
    },
  },
  {
    id: 'status',
    accessorKey: 'hasCompletedProfile',
    header: 'الحالة',
    cell: ({ getValue }) => {
      const isComplete = getValue() as boolean;
      return isComplete ? (
        <Badge
          variant="outline"
          className="gap-1 border-green-200 bg-green-50 text-xs text-green-700"
        >
          <CheckCircle className="h-3 w-3" />
          مكتمل
        </Badge>
      ) : (
        <Badge
          variant="outline"
          className="gap-1 border-yellow-200 bg-yellow-50 text-xs text-yellow-700"
        >
          <XCircle className="h-3 w-3" />
          غير مكتمل
        </Badge>
      );
    },
  },
  {
    id: 'actions',
    header: 'الإجراءات',
    cell: ({ row }) => {
      const member = row.original;
      return member.insuranceCardId ? (
        <Button
          variant="outline"
          size="sm"
          onClick={() => options.onPrintCard?.(member)}
          title="طباعة البطاقة"
          className="gap-1 text-xs"
        >
          <Printer className="h-3 w-3" />
          طباعة
        </Button>
      ) : (
        <span className="text-muted-foreground text-xs">-</span>
      );
    },
  },
];
```

**Key Patterns**:
- ✅ Smaller text sizes for sub-table (text-xs)
- ✅ Different colors for visual hierarchy (gray vs blue)
- ✅ Empty expander cell for alignment
- ✅ Indentation with padding (pr-8)
- ✅ Consistent column IDs with main table
- ✅ Same structure but different styling

---

## Example 6: TypeScript Types

**Source**: `types.ts`

```typescript
// Nested object interfaces from API response
export interface NestedEntity {
  id: string;
  name: string;
  nameArabic: string;
}

export interface FamilyMember {
  id: string;
  fullName: string;
  nationalNumber: string;
  civilId: string;
  gender: string;
  phoneNumber: string;
  insuranceCardId: string;
  organizationName: string;
  subOrganizationName?: string;
  relationshipType?: string;
  hasCompletedProfile?: boolean;
  // ... other fields
}

export interface FamilyGroup {
  familyHead: FamilyMember;
  familyMembers: FamilyMember[];
  totalMembers: number;
}

export interface PaginatedResponse {
  page: number;
  totalPages: number;
  count: number;
  totalCount: number;
  data: FamilyMember[];
}
```

**Key Patterns**:
- ✅ Clear interface names
- ✅ Optional fields with ?
- ✅ Nested types for complex data
- ✅ Separate API types from display types

---

## Common Patterns Summary

### 1. Column Configuration Pattern
```typescript
// Separate config file
export const createTableColumns = (options: Options): ColumnDef<Type>[] => [...]

// In component
const columns = useMemo(() => createTableColumns({ onAction }), [onAction]);
```

### 2. Pagination Setup Pattern
```typescript
const pagination = useMemo(
  () => ({
    page: currentPage - 1,  // Convert to 0-based
    size: pageSize,
    totalElements: totalCount,
    totalPages: totalPages,
    onPageChange: (page: number) => setCurrentPage(page + 1),  // Convert back
    onPageSizeChange: setPageSize,
  }),
  [currentPage, pageSize, totalCount, totalPages]
);
```

### 3. Expander Column Pattern
```typescript
{
  id: 'expander',
  header: () => null,
  cell: ({ row }) => (
    <Button
      variant="ghost"
      size="sm"
      onClick={() => {
        if (!row.getIsExpanded() && onRowExpand) {
          onRowExpand(row.original);
        }
        row.toggleExpanded();
      }}
    >
      {row.getIsExpanded() ? <ChevronUp /> : <ChevronDown />}
    </Button>
  ),
}
```

### 4. Sub-Row Rendering Pattern
```typescript
// Create renderer function
export const createRenderSubRow = (options) => {
  const columns = createSubColumns(options);
  
  return (parentRow) => {
    const subTable = useReactTable({ data: parentRow.children, columns, getCoreRowModel: getCoreRowModel() });
    return <SubRows table={subTable} />;
  };
};

// Use in meta
const tableMeta = useMemo(() => ({ renderSubRow: createRenderSubRow({ ... }) }), []);
```

### 5. Custom Styling Pattern
```typescript
export const createTableCustomStyle = () => ({
  tableHeaderClassName: 'bg-blue-50 sticky top-0 z-10',
  tableBodyRowClassName: 'hover:bg-gray-50',
  // ... other styles
});
```

### 6. Conditional Cell Rendering Pattern
```typescript
{
  id: 'actions',
  header: 'الإجراءات',
  cell: ({ row }) => {
    const item = row.original;
    return item.canPerformAction ? (
      <Button onClick={() => onAction(item)}>Action</Button>
    ) : (
      <span className="text-muted-foreground text-xs">-</span>
    );
  },
}
```

### 7. Badge Status Pattern
```typescript
{
  id: 'status',
  accessorKey: 'isComplete',
  header: 'الحالة',
  cell: ({ getValue }) => {
    const isComplete = getValue() as boolean;
    return (
      <Badge variant={isComplete ? 'default' : 'secondary'}>
        {isComplete ? 'مكتمل' : 'غير مكتمل'}
      </Badge>
    );
  },
}
```

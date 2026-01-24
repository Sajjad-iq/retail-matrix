# DataTable Quick Reference

## Minimal Working Example

```typescript
'use client';

import { DataTable } from '@/components/dataTable';
import { createTableColumns } from './config/table-config';
import { useMemo } from 'react';

export default function MyPage() {
  const { data, loading, currentPage, setCurrentPage, pageSize, setPageSize, totalCount, totalPages } = useData();
  
  const columns = useMemo(() => createTableColumns(), []);
  
  const pagination = useMemo(
    () => ({
      page: currentPage - 1,
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
        data={data}
        columns={columns}
        isLoading={loading}
        pagination={pagination}
      />
    </div>
  );
}
```

## Column Types Quick Reference

```typescript
import { ColumnDef } from '@tanstack/react-table';
import type { MyDataType } from './types';

export const createTableColumns = (): ColumnDef<MyDataType>[] => [
  // 1. Simple text column
  {
    id: 'name',
    accessorKey: 'fullName',
    header: 'الاسم',
    cell: ({ getValue }) => <span>{getValue() as string}</span>,
  },
  
  // 2. Computed column
  {
    id: 'fullName',
    accessorFn: (row) => `${row.firstName} ${row.lastName}`,
    header: 'الاسم الكامل',
    cell: ({ getValue }) => <span>{getValue() as string}</span>,
  },
  
  // 3. Badge/status column
  {
    id: 'status',
    accessorKey: 'isActive',
    header: 'الحالة',
    cell: ({ getValue }) => {
      const isActive = getValue() as boolean;
      return <Badge variant={isActive ? 'default' : 'secondary'}>{isActive ? 'نشط' : 'غير نشط'}</Badge>;
    },
  },
  
  // 4. Icon column
  {
    id: 'phone',
    accessorKey: 'phoneNumber',
    header: 'الهاتف',
    cell: ({ getValue }) => (
      <div className="flex items-center gap-2">
        <Phone className="h-4 w-4 text-muted-foreground" />
        {getValue() as string}
      </div>
    ),
  },
  
  // 5. Actions column
  {
    id: 'actions',
    header: 'الإجراءات',
    cell: ({ row }) => (
      <Button variant="outline" size="sm" onClick={() => onEdit(row.original)}>
        تعديل
      </Button>
    ),
  },
  
  // 6. Expander column (for expandable rows)
  {
    id: 'expander',
    header: () => null,
    cell: ({ row }) => (
      <Button variant="ghost" size="sm" onClick={() => row.toggleExpanded()}>
        {row.getIsExpanded() ? <ChevronUp /> : <ChevronDown />}
      </Button>
    ),
  },
];
```

## DataTable Props

```typescript
<DataTable
  // Required
  data={dataArray}                       // Array of data objects
  columns={columnsArray}                 // Column definitions
  
  // Pagination
  pagination={{
    page: 0,                             // Current page (0-based)
    size: 25,                            // Page size
    totalElements: 100,                  // Total items
    totalPages: 4,                       // Total pages
    onPageChange: (page) => {},          // Page change handler
    onPageSizeChange: (size) => {},      // Page size change handler
  }}
  
  // Optional
  isLoading={false}                      // Show loading state
  showToolbar={true}                     // Show/hide toolbar
  showPagination={true}                  // Show/hide pagination
  
  // Pagination configuration
  paginationConfig={{
    showPageInfo: true,                  // Show "Page X of Y"
    showSelectedCount: true,             // Show selected count
    showPageSize: true,                  // Show page size selector
    showFirstLastButtons: true,          // Show first/last buttons
    showPrevNextButtons: true,           // Show prev/next buttons
    showPageInput: true,                 // Show page jump input
  }}
  
  // Advanced
  meta={{
    renderSubRow: (row) => <SubRow />,   // Custom sub-row renderer
    tableHeaderClassName: '',            // Header styling
    tableBodyRowClassName: '',           // Body row styling
  }}
  
  // Other
  initialColumnVisibility={{}}           // Initial column visibility
  toolbar={(table) => <CustomToolbar />} // Custom toolbar
  onRetry={() => {}}                     // Retry on error
/>
```

## Column Definition Options

```typescript
{
  // Identity
  id: 'uniqueId',                        // Required: Unique identifier
  
  // Data access
  accessorKey: 'fieldName',              // Direct property access
  accessorFn: (row) => row.computed,     // Computed value function
  
  // Display
  header: 'Header Text',                 // Column header
  header: () => <CustomHeader />,        // Custom header component
  
  // Cell rendering
  cell: ({ getValue }) => getValue(),    // Simple cell
  cell: ({ row }) => row.original.name,  // Access full row
  cell: (info) => <CustomCell />,        // Custom cell component
  
  // Behavior
  enableSorting: false,                  // Disable sorting
  enableHiding: false,                   // Disable hiding
  size: 100,                             // Column size
}
```

## Pagination Pattern

```typescript
// In your data hook
const [currentPage, setCurrentPage] = useState(1);  // 1-based for API
const [pageSize, setPageSize] = useState(25);
const [totalCount, setTotalCount] = useState(0);
const [totalPages, setTotalPages] = useState(0);

// In your component
const pagination = useMemo(
  () => ({
    page: currentPage - 1,               // Convert to 0-based for DataTable
    size: pageSize,
    totalElements: totalCount,
    totalPages: totalPages,
    onPageChange: (page: number) => setCurrentPage(page + 1),  // Convert back to 1-based
    onPageSizeChange: setPageSize,
  }),
  [currentPage, pageSize, totalCount, totalPages]
);
```

## Expandable Rows Pattern

```typescript
// 1. Add expander column
const columns = [
  {
    id: 'expander',
    header: () => null,
    cell: ({ row }) => (
      <Button
        variant="ghost"
        size="sm"
        onClick={() => row.toggleExpanded()}
      >
        {row.getIsExpanded() ? <ChevronUp /> : <ChevronDown />}
      </Button>
    ),
  },
  // ... other columns
];

// 2. Create sub-row renderer
const renderSubRow = (parentRow: ParentType) => {
  return <SubTable data={parentRow.children} />;
};

// 3. Pass to meta
const tableMeta = useMemo(
  () => ({
    renderSubRow,
  }),
  []
);

// 4. Use in DataTable
<DataTable data={data} columns={columns} meta={tableMeta} />
```

## Custom Styling Pattern

```typescript
// In config file
export const createTableCustomStyle = () => ({
  tableHeaderClassName: 'bg-blue-50 sticky top-0 z-10',
  tableHeaderRowClassName: 'hover:bg-blue-50',
  tableHeaderCellClassName: 'text-center font-semibold text-blue-900',
  tableBodyRowClassName: 'hover:bg-gray-50',
  tableBodyCellClassName: 'text-center',
});

// In component
const tableMeta = useMemo(
  () => ({
    ...createTableCustomStyle(),
    renderSubRow,
  }),
  []
);
```

## Common Cell Patterns

```typescript
// Text with null handling
cell: ({ getValue }) => (getValue() as string) || '-'

// Number formatting
cell: ({ getValue }) => (getValue() as number).toLocaleString()

// Date formatting
cell: ({ getValue }) => new Date(getValue() as string).toLocaleDateString('ar-IQ')

// Conditional styling
cell: ({ getValue }) => {
  const value = getValue() as number;
  return (
    <span className={value > 0 ? 'text-green-600' : 'text-red-600'}>
      {value}
    </span>
  );
}

// Multiple values
cell: ({ row }) => (
  <div>
    <div className="font-medium">{row.original.name}</div>
    <div className="text-xs text-muted-foreground">{row.original.email}</div>
  </div>
)

// With icon
cell: ({ getValue }) => (
  <div className="flex items-center gap-2">
    <Icon className="h-4 w-4" />
    {getValue() as string}
  </div>
)

// Badge
cell: ({ getValue }) => {
  const status = getValue() as string;
  return (
    <Badge variant={status === 'active' ? 'default' : 'secondary'}>
      {status}
    </Badge>
  );
}

// Actions with condition
cell: ({ row }) => {
  const item = row.original;
  return item.canEdit ? (
    <Button onClick={() => onEdit(item)}>تعديل</Button>
  ) : (
    <span className="text-muted-foreground">-</span>
  );
}
```

## File Structure Template

```
feature/
├── page.tsx                           # Main page
├── useData.tsx                        # Data fetching hook
├── types.ts                           # TypeScript types
├── config/
│   ├── table-config.tsx              # Main table columns
│   └── sub-table-config.tsx          # Sub-table columns (optional)
└── components/
    └── sub-row-renderer.tsx          # Sub-row logic (optional)
```

## useData Hook Template

```typescript
// useData.tsx
import { useState, useEffect } from 'react';

export function useData() {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(25);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  
  const fetchData = async () => {
    setLoading(true);
    try {
      const response = await api.getData({ page: currentPage, size: pageSize });
      setData(response.data);
      setTotalCount(response.totalCount);
      setTotalPages(response.totalPages);
    } finally {
      setLoading(false);
    }
  };
  
  useEffect(() => {
    fetchData();
  }, [currentPage, pageSize]);
  
  return {
    data,
    loading,
    currentPage,
    setCurrentPage,
    pageSize,
    setPageSize,
    totalCount,
    totalPages,
    fetchData,
  };
}
```

## Checklist

- [ ] Created data types in `types.ts`
- [ ] Created column config in `config/table-config.tsx`
- [ ] Wrapped columns in `useMemo`
- [ ] Created pagination object with 0-based page index
- [ ] Wrapped pagination in `useMemo`
- [ ] Passed `isLoading` prop
- [ ] Set container height (e.g., `h-[650px]`)
- [ ] Used TypeScript generics (`ColumnDef<MyType>[]`)
- [ ] Handled null values in cells
- [ ] Applied responsive Tailwind classes

## Common Mistakes to Avoid

```typescript
// ❌ BAD: Columns not memoized
const columns = createTableColumns();

// ✅ GOOD: Columns memoized
const columns = useMemo(() => createTableColumns(), []);

// ❌ BAD: Wrong page indexing
page: currentPage

// ✅ GOOD: Correct 0-based indexing
page: currentPage - 1

// ❌ BAD: Inline column definitions
<DataTable columns={[{ id: 'name', ... }]} />

// ✅ GOOD: Separate config file
<DataTable columns={columns} />

// ❌ BAD: No loading state
<DataTable data={data} columns={columns} />

// ✅ GOOD: With loading state
<DataTable data={data} columns={columns} isLoading={loading} />

// ❌ BAD: No height constraint
<DataTable data={data} columns={columns} />

// ✅ GOOD: With fixed height
<div className="h-[650px]">
  <DataTable data={data} columns={columns} />
</div>
```

## Quick Actions Example

```typescript
// Actions column with multiple buttons
{
  id: 'actions',
  header: 'الإجراءات',
  cell: ({ row }) => {
    const item = row.original;
    return (
      <div className="flex items-center justify-center gap-2">
        <Button
          variant="outline"
          size="sm"
          onClick={() => onView(item)}
        >
          <Eye className="h-4 w-4" />
        </Button>
        <Button
          variant="outline"
          size="sm"
          onClick={() => onEdit(item)}
          disabled={!item.canEdit}
        >
          <Edit className="h-4 w-4" />
        </Button>
        <Button
          variant="destructive"
          size="sm"
          onClick={() => onDelete(item)}
          disabled={!item.canDelete}
        >
          <Trash className="h-4 w-4" />
        </Button>
      </div>
    );
  },
}
```

## Performance Tips

1. Always use `useMemo` for columns and pagination
2. Extract sub-row logic to separate files
3. Limit page size to reasonable numbers (10-100)
4. Debounce search inputs
5. Use server-side pagination, not client-side
6. Memoize callback functions passed to columns
7. Avoid inline complex JSX in cell renderers

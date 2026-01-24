# DataTable Pattern Rules

## Overview
DataTable is a custom table component built on top of TanStack Table (formerly React Table) v8. It provides a powerful, flexible way to display data with features like server-side pagination, expandable rows, custom styling, sorting, and column visibility.

## Core Principles

1. **TanStack Table Foundation**: Built on TanStack Table v8 API
2. **Type Safety**: Use TypeScript generics for data and column types
3. **Server-Side Pagination**: Manual pagination with server control
4. **Column Configuration**: Define columns in separate config files
5. **Extensible Meta**: Use table meta for custom features like sub-rows
6. **Responsive Design**: Mobile-first with Tailwind classes

## Basic DataTable Structure

### Simple Table Pattern

```typescript
'use client';

import { DataTable } from '@/components/dataTable';
import { createTableColumns } from './config/table-config';
import { useMemo } from 'react';
import type { MyDataType } from './types';

export default function MyTablePage() {
  const { data, loading, currentPage, pageSize, totalCount, totalPages } = useData();
  
  // Define columns
  const columns = useMemo(
    () => createTableColumns(),
    []
  );
  
  // Pagination state
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
        data={data}
        columns={columns}
        isLoading={loading}
        pagination={pagination}
        showToolbar={false}
        showPagination={true}
      />
    </div>
  );
}
```

## Column Definitions

### Column Configuration File Pattern

```typescript
// config/table-config.tsx
'use client';

import { ColumnDef } from '@tanstack/react-table';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Phone, Mail, Printer } from 'lucide-react';
import type { MyDataType } from '../types';

interface ColumnsOptions {
  onEdit?: (row: MyDataType) => void;
  onDelete?: (row: MyDataType) => void;
  onPrint?: (row: MyDataType) => void;
}

export const createTableColumns = (
  options: ColumnsOptions = {}
): ColumnDef<MyDataType>[] => [
  {
    id: 'name',
    accessorKey: 'fullName',
    header: 'الاسم الكامل',
    cell: ({ getValue }) => (
      <span className="font-medium">{getValue() as string}</span>
    ),
  },
  {
    id: 'email',
    accessorKey: 'email',
    header: 'البريد الإلكتروني',
    cell: ({ getValue }) => (
      <div className="flex items-center gap-2">
        <Mail className="h-4 w-4 text-muted-foreground" />
        {getValue() as string}
      </div>
    ),
  },
  {
    id: 'phone',
    accessorKey: 'phoneNumber',
    header: 'رقم الهاتف',
    cell: ({ getValue }) => (
      <div className="flex items-center gap-2">
        <Phone className="h-4 w-4 text-muted-foreground" />
        {getValue() as string}
      </div>
    ),
  },
  {
    id: 'status',
    accessorKey: 'isActive',
    header: 'الحالة',
    cell: ({ getValue }) => {
      const isActive = getValue() as boolean;
      return (
        <Badge variant={isActive ? 'default' : 'secondary'}>
          {isActive ? 'نشط' : 'غير نشط'}
        </Badge>
      );
    },
  },
  {
    id: 'actions',
    header: 'الإجراءات',
    cell: ({ row }) => (
      <div className="flex items-center gap-2">
        <Button
          variant="outline"
          size="sm"
          onClick={() => options.onPrint?.(row.original)}
        >
          <Printer className="h-4 w-4" />
          طباعة
        </Button>
      </div>
    ),
  },
];
```

## Column Types & Patterns

### 1. Simple Data Column
```typescript
{
  id: 'name',
  accessorKey: 'fullName',  // Direct property access
  header: 'الاسم',
  cell: ({ getValue }) => <span>{getValue() as string}</span>,
}
```

### 2. Computed Data Column
```typescript
{
  id: 'fullName',
  accessorFn: (row) => `${row.firstName} ${row.lastName}`,  // Computed value
  header: 'الاسم الكامل',
  cell: ({ getValue }) => <span>{getValue() as string}</span>,
}
```

### 3. Complex Cell Rendering
```typescript
{
  id: 'user',
  accessorKey: 'fullName',
  header: 'المستخدم',
  cell: ({ row }) => {
    const user = row.original;
    return (
      <div className="flex items-center gap-2">
        <Avatar src={user.avatar} />
        <div>
          <div className="font-medium">{user.fullName}</div>
          <div className="text-xs text-muted-foreground">{user.email}</div>
        </div>
      </div>
    );
  },
}
```

### 4. Badge/Status Column
```typescript
{
  id: 'status',
  accessorKey: 'status',
  header: 'الحالة',
  cell: ({ getValue }) => {
    const status = getValue() as string;
    const variants = {
      'active': 'default',
      'pending': 'secondary',
      'inactive': 'destructive',
    };
    return <Badge variant={variants[status]}>{status}</Badge>;
  },
}
```

### 5. Actions Column
```typescript
{
  id: 'actions',
  header: 'الإجراءات',
  cell: ({ row }) => {
    const item = row.original;
    return (
      <div className="flex items-center gap-2">
        <Button
          variant="outline"
          size="sm"
          onClick={() => options.onEdit?.(item)}
        >
          تعديل
        </Button>
        <Button
          variant="destructive"
          size="sm"
          onClick={() => options.onDelete?.(item)}
        >
          حذف
        </Button>
      </div>
    );
  },
}
```

### 6. Icon Column
```typescript
{
  id: 'phone',
  accessorKey: 'phoneNumber',
  header: 'الهاتف',
  cell: ({ getValue }) => (
    <div className="flex items-center justify-center gap-1">
      <Phone className="h-3 w-3 text-muted-foreground" />
      {getValue() as string || '-'}
    </div>
  ),
}
```

## DataTable API

### Root Component Props

```typescript
<DataTable
  data={dataArray}                    // Required: Array of data
  columns={columnsArray}              // Required: Column definitions
  isLoading={boolean}                 // Optional: Loading state
  pagination={paginationState}        // Optional: Server pagination
  meta={tableMetaObject}              // Optional: Custom table options
  showToolbar={boolean}               // Optional: Show/hide toolbar (default: true)
  showPagination={boolean}            // Optional: Show/hide pagination (default: true)
  paginationConfig={configObject}     // Optional: Pagination features config
  initialColumnVisibility={{}}        // Optional: Initial column visibility
  toolbar={(table) => ReactNode}      // Optional: Custom toolbar
  onRetry={() => void}                // Optional: Retry on error
/>
```

### Pagination State

```typescript
const pagination = {
  page: number,                       // Current page (0-based)
  size: number,                       // Page size
  totalElements: number,              // Total items count
  totalPages: number,                 // Total pages count
  onPageChange?: (page: number) => void,     // Page change handler
  onPageSizeChange?: (size: number) => void, // Page size change handler
};
```

### Pagination Configuration

```typescript
const paginationConfig = {
  showPageInfo: boolean,              // Show "Page X of Y" (default: true)
  showSelectedCount: boolean,         // Show selected rows count (default: true)
  showPageSize: boolean,              // Show page size selector (default: true)
  showFirstLastButtons: boolean,      // Show first/last page buttons (default: true)
  showPrevNextButtons: boolean,       // Show prev/next buttons (default: true)
  showPageInput: boolean,             // Show page jump input (default: true)
};
```

### Table Meta

```typescript
const tableMeta = {
  renderSubRow?: (row: TData, index: number) => ReactNode,
  tableHeaderClassName?: string,
  tableHeaderRowClassName?: string,
  tableHeaderCellClassName?: string,
  tableBodyRowClassName?: string,
  tableBodyCellClassName?: string,
};
```

## Advanced Features

### 1. Expandable Rows with Sub-Rows

**Step 1: Create Expander Column**
```typescript
// config/table-config.tsx
{
  id: 'expander',
  header: () => null,
  cell: ({ row }) => (
    <Button
      variant="ghost"
      size="sm"
      onClick={() => {
        if (!row.getIsExpanded() && options.onRowExpand) {
          options.onRowExpand(row.original);
        }
        row.toggleExpanded();
      }}
      className="h-6 w-6 p-0"
    >
      {row.getIsExpanded() ? (
        <ChevronUp className="h-4 w-4" />
      ) : (
        <ChevronDown className="h-4 w-4" />
      )}
    </Button>
  ),
}
```

**Step 2: Create Sub-Row Renderer**
```typescript
// components/sub-row-renderer.tsx
import { ColumnDef, flexRender, getCoreRowModel, useReactTable } from '@tanstack/react-table';
import { TableRow, TableCell } from '@/components/ui/table';

export const createRenderSubRow = (options: Options) => {
  const columns = createSubRowColumns(options);
  
  return (parentRow: ParentType) => {
    const subTable = useReactTable({
      data: parentRow.children,
      columns,
      getCoreRowModel: getCoreRowModel(),
    });
    
    const subRows = subTable.getRowModel().rows;
    
    if (subRows.length === 0) {
      return (
        <TableRow>
          <TableCell colSpan={columns.length} className="text-center">
            لا توجد بيانات
          </TableCell>
        </TableRow>
      );
    }
    
    return (
      <>
        {subRows.map((row) => (
          <TableRow key={row.id} className="bg-gray-50">
            {row.getVisibleCells().map((cell) => (
              <TableCell key={cell.id}>
                {flexRender(cell.column.columnDef.cell, cell.getContext())}
              </TableCell>
            ))}
          </TableRow>
        ))}
      </>
    );
  };
};
```

**Step 3: Pass to DataTable**
```typescript
const tableMeta = useMemo(
  () => ({
    renderSubRow: createRenderSubRow({ onAction: handleAction }),
  }),
  [handleAction]
);

<DataTable data={data} columns={columns} meta={tableMeta} />
```

### 2. Custom Styling

```typescript
export const createTableCustomStyle = () => ({
  tableHeaderClassName: 'bg-blue-50 border-border sticky top-0 z-10 border-b-2',
  tableHeaderRowClassName: 'border-0 hover:bg-blue-50',
  tableHeaderCellClassName: 'text-center font-semibold whitespace-nowrap text-blue-900',
  tableBodyRowClassName: 'border-b-2 border-blue-300 bg-blue-100 font-semibold hover:bg-blue-200',
  tableBodyCellClassName: 'text-center whitespace-nowrap',
});

const tableMeta = useMemo(
  () => ({
    ...createTableCustomStyle(),
    renderSubRow: createRenderSubRow(),
  }),
  []
);
```

### 3. Loading States

```typescript
// Automatic skeleton loading
<DataTable
  data={data}
  columns={columns}
  isLoading={loading}  // Shows skeleton rows
/>
```

### 4. Conditional Rendering in Cells

```typescript
{
  id: 'actions',
  header: 'الإجراءات',
  cell: ({ row }) => {
    const item = row.original;
    return item.canEdit ? (
      <Button onClick={() => onEdit(item)}>تعديل</Button>
    ) : (
      <span className="text-muted-foreground text-xs">غير متاح</span>
    );
  },
}
```

### 5. Custom Cell Styling

```typescript
{
  id: 'amount',
  accessorKey: 'amount',
  header: 'المبلغ',
  cell: ({ getValue }) => {
    const amount = getValue() as number;
    return (
      <span className={cn(
        'font-semibold',
        amount > 0 ? 'text-green-600' : 'text-red-600'
      )}>
        {amount.toLocaleString()} IQD
      </span>
    );
  },
}
```

## Integration Patterns

### With useData Hook

```typescript
// useData.tsx
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
      const response = await api.getData({
        page: currentPage,
        size: pageSize,
      });
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

// page.tsx
export default function Page() {
  const {
    data,
    loading,
    currentPage,
    setCurrentPage,
    pageSize,
    setPageSize,
    totalCount,
    totalPages,
  } = useData();
  
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
  
  return <DataTable data={data} columns={columns} pagination={pagination} isLoading={loading} />;
}
```

### With React Query

```typescript
'use client';

import { useQuery } from '@tanstack/react-query';
import { DataTable } from '@/components/dataTable';

export default function Page() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  
  const { data, isLoading } = useQuery({
    queryKey: ['data', page, pageSize],
    queryFn: () => fetchData({ page, size: pageSize }),
  });
  
  const pagination = useMemo(
    () => ({
      page,
      size: pageSize,
      totalElements: data?.totalCount ?? 0,
      totalPages: data?.totalPages ?? 0,
      onPageChange: setPage,
      onPageSizeChange: setPageSize,
    }),
    [page, pageSize, data]
  );
  
  return (
    <DataTable
      data={data?.items ?? []}
      columns={columns}
      isLoading={isLoading}
      pagination={pagination}
    />
  );
}
```

## Common Patterns

### Pattern 1: Master-Detail Table (Expandable)

```typescript
// Main columns with expander
const mainColumns = [
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

// Sub-row renderer
const renderSubRow = (family: FamilyGroup) => {
  return <FamilyMembersTable members={family.members} />;
};

// Usage
<DataTable
  data={families}
  columns={mainColumns}
  meta={{ renderSubRow }}
/>
```

### Pattern 2: Table with Filters

```typescript
export default function FilteredTablePage() {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  
  const { data, loading, pagination } = useFilteredData({
    searchTerm,
    statusFilter,
  });
  
  return (
    <Card>
      <CardContent>
        {/* Filters */}
        <div className="mb-4 flex gap-4">
          <Input
            placeholder="البحث..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
          <Select value={statusFilter} onValueChange={setStatusFilter}>
            <SelectTrigger>
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">الكل</SelectItem>
              <SelectItem value="active">نشط</SelectItem>
              <SelectItem value="inactive">غير نشط</SelectItem>
            </SelectContent>
          </Select>
        </div>
        
        {/* Table */}
        <DataTable
          data={data}
          columns={columns}
          isLoading={loading}
          pagination={pagination}
        />
      </CardContent>
    </Card>
  );
}
```

### Pattern 3: Table with Actions Bar

```typescript
export default function TableWithActions() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>إدارة البيانات</CardTitle>
      </CardHeader>
      <CardContent>
        {/* Actions */}
        <div className="mb-4 flex justify-between">
          <Button onClick={handleAdd}>
            <Plus className="mr-2 h-4 w-4" />
            إضافة جديد
          </Button>
          <div className="flex gap-2">
            <Button variant="outline" onClick={handleExport}>
              <Download className="mr-2 h-4 w-4" />
              تصدير
            </Button>
            <Button variant="outline" onClick={handlePrint}>
              <Printer className="mr-2 h-4 w-4" />
              طباعة
            </Button>
          </div>
        </div>
        
        {/* Table */}
        <div className="h-[600px]">
          <DataTable
            data={data}
            columns={columns}
            isLoading={loading}
            pagination={pagination}
          />
        </div>
      </CardContent>
    </Card>
  );
}
```

## File Organization

```
app/(feature)/
├── page.tsx                        # Main page with DataTable
├── useData.tsx                     # Data fetching hook
├── types.ts                        # TypeScript types
├── config/
│   ├── table-config.tsx           # Main table columns
│   └── sub-table-config.tsx       # Sub-table columns (if needed)
└── components/
    └── sub-row-renderer.tsx       # Sub-row rendering logic
```

## Best Practices

### ✅ DO

```typescript
// ✅ Use useMemo for columns and pagination
const columns = useMemo(() => createColumns({ onEdit }), [onEdit]);
const pagination = useMemo(() => ({...}), [currentPage, pageSize]);

// ✅ Separate column definitions in config files
// config/table-config.tsx
export const createTableColumns = (options) => [...];

// ✅ Use TypeScript generics
const columns: ColumnDef<MyDataType>[] = [...];

// ✅ Use 0-based page indexing for DataTable
page: currentPage - 1  // If your API uses 1-based

// ✅ Handle loading states
<DataTable data={data} columns={columns} isLoading={loading} />

// ✅ Use accessorFn for computed values
accessorFn: (row) => `${row.firstName} ${row.lastName}`

// ✅ Extract sub-row logic to separate files
const renderSubRow = createRenderSubRow({ onAction });

// ✅ Use conditional rendering in cells
cell: ({ row }) => row.original.hasCard ? <Button>طباعة</Button> : <span>-</span>

// ✅ Apply responsive classes
className="h-9 text-xs sm:h-10 sm:text-sm"

// ✅ Set fixed height for table container
<div className="h-[650px]">
  <DataTable ... />
</div>
```

### ❌ DON'T

```typescript
// ❌ Don't create columns inline
<DataTable columns={[{ id: 'name', ... }]} />  // Bad

// ❌ Don't forget useMemo for expensive computations
const columns = createColumns();  // Bad - recreates every render

// ❌ Don't use any type
const columns: ColumnDef<any>[] = [...];  // Bad

// ❌ Don't mix 0-based and 1-based indexing
page: currentPage  // Bad if API is 1-based

// ❌ Don't inline complex sub-row logic in meta
meta={{ renderSubRow: (row) => <ComplexComponent {...row} /> }}  // Bad

// ❌ Don't skip loading states
<DataTable data={data} columns={columns} />  // Bad - no loading prop

// ❌ Don't create large inline JSX in cells
cell: ({ row }) => (
  <div>
    {/* 50 lines of JSX */}
  </div>
)  // Bad - extract to component

// ❌ Don't forget null checks
cell: ({ getValue }) => getValue().toString()  // Bad - might be null
```

## Performance Tips

1. **Use useMemo**: Wrap columns and pagination in `useMemo`
2. **Separate Column Config**: Define columns in separate files
3. **Lazy Load Sub-Rows**: Fetch sub-row data only when expanded
4. **Limit Page Size**: Don't load too many rows at once
5. **Debounce Search**: Use debounce for search inputs
6. **Optimize Callbacks**: Memoize callback functions passed to columns

## Common Issues & Solutions

### Issue: Table not updating after data changes
**Solution**: Ensure you're passing new array references, not mutating existing arrays

### Issue: Pagination not working
**Solution**: Check that page indexing is correct (0-based for DataTable)

### Issue: Sub-rows not rendering
**Solution**: Ensure `renderSubRow` is in table meta and `getExpandedRowModel` is enabled

### Issue: Columns recreating on every render
**Solution**: Wrap column creation in `useMemo` with proper dependencies

### Issue: Loading state not showing
**Solution**: Pass `isLoading` prop to DataTable component

### Issue: Custom styling not applying
**Solution**: Pass styling classes via `meta` object, not directly to DataTable

## TypeScript Types Reference

```typescript
import { ColumnDef } from '@tanstack/react-table';

// Data type
interface MyDataType {
  id: string;
  name: string;
  email: string;
  // ... other fields
}

// Column definition
const columns: ColumnDef<MyDataType>[] = [...];

// Pagination state
interface PaginationState {
  page: number;
  size: number;
  totalElements: number;
  totalPages: number;
  onPageChange?: (page: number) => void;
  onPageSizeChange?: (size: number) => void;
}

// Table meta
interface DataTableMeta<TData> {
  renderSubRow?: (row: TData, index: number) => React.ReactNode;
  tableHeaderClassName?: string;
  tableHeaderRowClassName?: string;
  tableHeaderCellClassName?: string;
  tableBodyRowClassName?: string;
  tableBodyCellClassName?: string;
}
```

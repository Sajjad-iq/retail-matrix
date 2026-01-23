import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from '@/app/components/ui/pagination';

interface DataTablePaginationControlsProps {
  currentPage: number;
  totalPages: number;
  canPreviousPage: boolean;
  canNextPage: boolean;
  onPageChange?: (page: number) => void;
}

export function DataTablePaginationControls({
  currentPage,
  totalPages,
  canPreviousPage,
  canNextPage,
  onPageChange,
}: DataTablePaginationControlsProps) {
  // Generate page numbers to display
  const getPageNumbers = () => {
    const pages: (number | 'ellipsis')[] = [];
    const maxVisible = 5;

    if (totalPages <= maxVisible) {
      // Show all pages if total is less than max visible
      for (let i = 0; i < totalPages; i++) {
        pages.push(i);
      }
    } else {
      // Always show first page
      pages.push(0);

      if (currentPage > 2) {
        pages.push('ellipsis');
      }

      // Show pages around current page
      const start = Math.max(1, currentPage - 1);
      const end = Math.min(totalPages - 2, currentPage + 1);

      for (let i = start; i <= end; i++) {
        pages.push(i);
      }

      if (currentPage < totalPages - 3) {
        pages.push('ellipsis');
      }

      // Always show last page
      pages.push(totalPages - 1);
    }

    return pages;
  };

  const pages = getPageNumbers();

  return (
    <Pagination>
      <PaginationContent>
        <PaginationItem>
          <PaginationPrevious
            href="#"
            onClick={(e) => {
              e.preventDefault();
              if (canPreviousPage) {
                onPageChange?.(currentPage - 1);
              }
            }}
            className={!canPreviousPage ? 'pointer-events-none opacity-50' : ''}
            size="default"
          />
        </PaginationItem>

        {pages.map((page, index) => (
          <PaginationItem key={`${page}-${index}`}>
            {page === 'ellipsis' ? (
              <PaginationEllipsis />
            ) : (
              <PaginationLink
                href="#"
                onClick={(e) => {
                  e.preventDefault();
                  onPageChange?.(page);
                }}
                isActive={currentPage === page}
                size="icon"
              >
                {page + 1}
              </PaginationLink>
            )}
          </PaginationItem>
        ))}

        <PaginationItem>
          <PaginationNext
            href="#"
            onClick={(e) => {
              e.preventDefault();
              if (canNextPage) {
                onPageChange?.(currentPage + 1);
              }
            }}
            className={!canNextPage ? 'pointer-events-none opacity-50' : ''}
            size="default"
          />
        </PaginationItem>
      </PaginationContent>
    </Pagination>
  );
}

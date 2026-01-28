import { ReactNode, useState } from 'react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from '@/app/components/ui/dialog';
import { FormBuilder } from '@/app/components/form';
import { updateCurrencySchema, UpdateCurrencyFormValues } from '../lib/validations';
import { useUpdateCurrency } from '../hooks/useCurrencyActions';
import { CurrencyDto, CurrencyStatus } from '../lib/types';

interface EditCurrencyDialogProps {
    currency: CurrencyDto;
    children: ReactNode;
}

export function EditCurrencyDialog({ currency, children }: EditCurrencyDialogProps) {
    const [open, setOpen] = useState(false);
    const { mutateAsync: updateCurrency } = useUpdateCurrency();

    const handleSubmit = async (data: UpdateCurrencyFormValues) => {
        await updateCurrency({ id: currency.id, data });
        setOpen(false);
    };

    const statusOptions = [
        { label: 'نشط', value: CurrencyStatus.Active.toString() },
        { label: 'غير نشط', value: CurrencyStatus.Inactive.toString() },
    ];

    return (
        <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>{children}</DialogTrigger>
            <DialogContent className="max-w-md">
                <DialogHeader>
                    <DialogTitle>تعديل العملة: {currency.code}</DialogTitle>
                </DialogHeader>

                <FormBuilder
                    schema={updateCurrencySchema}
                    onSubmit={handleSubmit}
                    defaultValues={{
                        name: currency.name,
                        symbol: currency.symbol,
                        exchangeRate: currency.exchangeRate,
                        status: currency.status,
                    }}
                >
                    <FormBuilder.Text
                        name="name"
                        label="اسم العملة"
                        placeholder="دينار عراقي"
                        required
                    />

                    <FormBuilder.Text
                        name="symbol"
                        label="رمز العملة"
                        placeholder="IQD"
                        required
                    />

                    <FormBuilder.Number
                        name="exchangeRate"
                        label="سعر الصرف"
                        placeholder="1.0000"
                        step={0.0001}
                        min={0.0001}
                        required
                    />

                    <FormBuilder.Select
                        name="status"
                        label="الحالة"
                        options={statusOptions}
                        required
                    />

                    <FormBuilder.Submit>تحديث</FormBuilder.Submit>
                </FormBuilder>
            </DialogContent>
        </Dialog>
    );
}

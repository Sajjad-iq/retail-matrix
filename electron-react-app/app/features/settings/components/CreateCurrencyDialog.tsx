import { ReactNode, useState } from 'react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from '@/app/components/ui/dialog';
import { FormBuilder } from '@/app/components/form';
import { currencySchema, CurrencyFormValues } from '../lib/validations';
import { useCreateCurrency } from '../hooks/useCurrencyActions';
import { CurrencyStatus } from '../lib/types';

interface CreateCurrencyDialogProps {
    children: ReactNode;
}

export function CreateCurrencyDialog({ children }: CreateCurrencyDialogProps) {
    const [open, setOpen] = useState(false);
    const { mutateAsync: createCurrency } = useCreateCurrency();

    const handleSubmit = async (data: CurrencyFormValues) => {
        await createCurrency(data);
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
                    <DialogTitle>إضافة عملة جديدة</DialogTitle>
                </DialogHeader>

                <FormBuilder
                    schema={currencySchema}
                    onSubmit={handleSubmit}
                    defaultValues={{
                        code: '',
                        name: '',
                        symbol: '',
                        exchangeRate: 1,
                        isBaseCurrency: false,
                        status: CurrencyStatus.Active,
                    }}
                >
                    <FormBuilder.Text
                        name="code"
                        label="رمز العملة (مثل: USD, IQD)"
                        placeholder="IQD"
                        required
                    />

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

                    <FormBuilder.Checkbox
                        name="isBaseCurrency"
                        label="عملة أساسية"
                        description="العملة الأساسية تستخدم في جميع الحسابات"
                    />

                    <FormBuilder.Select
                        name="status"
                        label="الحالة"
                        options={statusOptions}
                        required
                    />

                    <FormBuilder.Submit>إضافة</FormBuilder.Submit>
                </FormBuilder>
            </DialogContent>
        </Dialog>
    );
}

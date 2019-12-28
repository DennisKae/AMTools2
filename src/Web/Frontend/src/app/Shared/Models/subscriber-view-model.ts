import { AvailabilityStatusViewModel } from './availability-status-view-model';
import { QualificationSettingViewModel } from './Settings/qualification-setting-view-model';

export class SubscriberViewModel {
    Issi: string;

    Name: string;

    Qualification: string;

    Qualifications: QualificationSettingViewModel[];

    AvailabilityStatus: AvailabilityStatusViewModel;
}

import { QualificationSettingViewModel } from 'src/app/Shared/Models/Settings/qualification-setting-view-model';
import { SubscriberViewModel } from 'src/app/Shared/Models/subscriber-view-model';

export class QualificationDisplayModel {

    Qualification: QualificationSettingViewModel;

    Subscribers: SubscriberViewModel[];

    AnzahlVerfuegbar: number;
}

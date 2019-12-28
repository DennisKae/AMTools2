import { AppLogSeverity } from './app-log-severity';

export class AppLog {
    Timestamp: Date | string;

    Severity: AppLogSeverity;

    Message: string;

    ApplicationPart: string;
    
    BatchCommand: string;
}

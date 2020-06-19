

export class Constants {

    public static get genericServiceErrorMessage(): string { return "FaktoryWize Service not accessible" };
    public static get recordAddedMessage(): string { return "Record added successfully." };
    public static get recordDeletedMessage(): string { return "Record deleted successfully." };
    public static get recordDeactivatedMessage(): string { return "Record deactivated successfully." };
    public static get recordActivatedMessage(): string { return "Record activated successfully." };
  public static get recordUnlockedMessage(): string { return "User Unlocked successfully." };
  public static get recordlockedMessage(): string { return "User locked successfully." };
    public static get recordUpdatedMessage(): string { return "Record updated successfully." };
    public static get changePasswordMessage(): string { return "Password changed successfully." };
    public static get setPasswordMessage(): string { return "Password set successfully." };
    public static get sentPasswordMessage(): string { return "Password link sent successfully on email." };
    public static get msgBoxSuccess(): string { return "success" };
    public static get msgBoxError(): string { return "error" };
    public static get msgBoxWarning(): string { return "warning" };
    public static get actionButtonOk(): string { return "ok" };
    public static get inputMinLenth(): number { return 3 };
    public static get ouShortCodeMaxLenth(): number { return 3 };
    public static get inputMaxLenth(): number { return 50 };
    public static get txtAreaMaxLenth(): number { return 255 };
    public static get characterPattern(): string { return "^[a-zA-Z]+$" };
    public static get numericAlphaSpecialPattern(): string { return "^[A-Za-z0-9_@./#&+-]+$" };
    public static get onlyAlphabetsWithSpaceQuoteHyphen(): string { return "[a-zA-Z-0-9' ]*" };
    public static get onlyAlphabetsWithDot(): string { return "[0-9.]*" };

    public static get DefaultPageSize(): number { return 0 };
    public static get DefaultPageIndex(): number { return 0 };
    public static get PartitionKey(): string { return 'PartitionKey' };
    public static get Ascending(): number { return 1 };
    public static get Descending(): number { return 2 };
    public static get Contains(): number { return 1 };
    public static get Exact(): number { return 2 };
    public static get UserName(): string { return 'FirstName' };
    public static get Name(): string { return 'Name' };
    public static get UserCode(): string { return 'UserCode' };
    public static get ResourceLoadingFailedMsg(): string { return "Resouce Loading Failed.." };
    public static get EntityName(): string { return "EntityName" };
    public static get smtpTestedMessage(): string { return "Tested Successfully" };
    public static get StartDate(): string { return "StartDate" };


}

USE RecommendationEngine1; 

GO

CREATE TRIGGER trg_DeleteOrderItemsOnRecommendedMenuDelete
ON RecommendedMenus
FOR DELETE
AS
BEGIN
    DELETE FROM OrderItems
    WHERE MenuId IN (SELECT MenuId FROM DELETED);
END;
GO

